#!/usr/bin/env -S xcrun swift

import AppKit
import Darwin
import Foundation

struct Item {
    let path: String
    let label: String
}

private func fail(_ message: String) -> Never {
    FileHandle.standardError.write(Data("error: \(message)\n".utf8))
    exit(2)
}

private let usage = """
Usage:
  xcrun swift scripts/create-contact-sheet.swift \\
    --output <sheet.png> [--columns <count>] \\
    --item <image> <label> [--item <image> <label> ...]

The macOS AppKit compositor preserves each source image pixel-for-pixel without
cropping, adds a 48-pixel label strip, and writes a PNG suitable for review.
All input images in one sheet must have the same dimensions.
"""

var outputPath: String?
var columnCount = 3
var items: [Item] = []
let arguments = Array(CommandLine.arguments.dropFirst())
var argumentIndex = 0

while argumentIndex < arguments.count {
    switch arguments[argumentIndex] {
    case "--help", "-h":
        print(usage)
        exit(0)
    case "--output" where argumentIndex + 1 < arguments.count:
        outputPath = arguments[argumentIndex + 1]
        argumentIndex += 2
    case "--columns" where argumentIndex + 1 < arguments.count:
        guard let value = Int(arguments[argumentIndex + 1]), value > 0 else {
            fail("--columns must be a positive integer")
        }
        columnCount = value
        argumentIndex += 2
    case "--item" where argumentIndex + 2 < arguments.count:
        items.append(Item(path: arguments[argumentIndex + 1], label: arguments[argumentIndex + 2]))
        argumentIndex += 3
    default:
        fail("unknown or incomplete argument '\(arguments[argumentIndex])'\n\n\(usage)")
    }
}

guard let outputPath else {
    fail("--output is required\n\n\(usage)")
}
guard !items.isEmpty else {
    fail("at least one --item is required\n\n\(usage)")
}

let loadedItems: [(image: NSImage, pixelWidth: Int, pixelHeight: Int, label: String)] = items.map { item in
    guard let data = FileManager.default.contents(atPath: item.path),
          let representation = NSBitmapImageRep(data: data),
          representation.pixelsWide > 0,
          representation.pixelsHigh > 0 else {
        fail("could not load '\(item.path)'")
    }
    let pixelSize = NSSize(width: representation.pixelsWide, height: representation.pixelsHigh)
    let image = NSImage(size: pixelSize)
    image.addRepresentation(representation)
    return (image, representation.pixelsWide, representation.pixelsHigh, item.label)
}

let sourceWidth = loadedItems[0].pixelWidth
let sourceHeight = loadedItems[0].pixelHeight
for (index, item) in loadedItems.enumerated()
    where item.pixelWidth != sourceWidth || item.pixelHeight != sourceHeight {
    fail("input image \(index + 1) is \(item.pixelWidth)x\(item.pixelHeight), expected \(sourceWidth)x\(sourceHeight)")
}

let labelHeight = 48
let cellWidth = sourceWidth
let cellHeight = sourceHeight + labelHeight
let rowCount = (loadedItems.count + columnCount - 1) / columnCount
let canvasWidth = cellWidth * columnCount
let canvasHeight = cellHeight * rowCount
guard let bitmap = NSBitmapImageRep(
    bitmapDataPlanes: nil,
    pixelsWide: canvasWidth,
    pixelsHigh: canvasHeight,
    bitsPerSample: 8,
    samplesPerPixel: 4,
    hasAlpha: true,
    isPlanar: false,
    colorSpaceName: .deviceRGB,
    bytesPerRow: 0,
    bitsPerPixel: 0),
    let context = NSGraphicsContext(bitmapImageRep: bitmap) else {
    fail("could not allocate the contact-sheet pixel canvas")
}

NSGraphicsContext.saveGraphicsState()
NSGraphicsContext.current = context
context.imageInterpolation = .none

NSColor(calibratedWhite: 0.035, alpha: 1.0).setFill()
NSRect(x: 0, y: 0, width: CGFloat(canvasWidth), height: CGFloat(canvasHeight)).fill()

let labelStyle: [NSAttributedString.Key: Any] = [
    .font: NSFont.monospacedSystemFont(ofSize: 20, weight: .medium),
    .foregroundColor: NSColor.white,
]

for (index, item) in loadedItems.enumerated() {
    let column = index % columnCount
    let rowFromTop = index / columnCount
    let cellX = CGFloat(column * cellWidth)
    let cellY = CGFloat((rowCount - rowFromTop - 1) * cellHeight)
    item.image.draw(
        in: NSRect(
            x: cellX,
            y: cellY + CGFloat(labelHeight),
            width: CGFloat(cellWidth),
            height: CGFloat(sourceHeight)),
        from: .zero,
        operation: .copy,
        fraction: 1.0)
    (item.label as NSString).draw(
        at: NSPoint(x: cellX + 16, y: cellY + 14),
        withAttributes: labelStyle)
}

NSGraphicsContext.restoreGraphicsState()
guard let png = bitmap.representation(using: .png, properties: [:]) else {
    fail("could not encode the contact sheet")
}

do {
    try png.write(to: URL(fileURLWithPath: outputPath), options: .atomic)
} catch {
    fail("could not write '\(outputPath)': \(error.localizedDescription)")
}

print("Wrote \(outputPath) (\(bitmap.pixelsWide)x\(bitmap.pixelsHigh), \(loadedItems.count) items)")

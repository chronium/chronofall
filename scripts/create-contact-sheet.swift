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

The macOS AppKit compositor preserves each source image without cropping, adds a
48-point label strip, and writes a 2x PNG suitable for project-history review.
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

let loadedItems: [(image: NSImage, label: String)] = items.map { item in
    guard let image = NSImage(contentsOfFile: item.path) else {
        fail("could not load '\(item.path)'")
    }
    return (image, item.label)
}

let sourceSize = loadedItems[0].image.size
guard sourceSize.width > 0, sourceSize.height > 0 else {
    fail("the first input image has invalid dimensions")
}
for (index, item) in loadedItems.enumerated() where item.image.size != sourceSize {
    fail("input image \(index + 1) is \(item.image.size), expected \(sourceSize)")
}

let labelHeight: CGFloat = 48
let cellWidth = sourceSize.width
let cellHeight = sourceSize.height + labelHeight
let rowCount = (loadedItems.count + columnCount - 1) / columnCount
let canvasSize = NSSize(
    width: cellWidth * CGFloat(columnCount),
    height: cellHeight * CGFloat(rowCount))
let canvas = NSImage(size: canvasSize)
canvas.lockFocus()

NSColor(calibratedWhite: 0.035, alpha: 1.0).setFill()
NSRect(origin: .zero, size: canvasSize).fill()

let labelStyle: [NSAttributedString.Key: Any] = [
    .font: NSFont.monospacedSystemFont(ofSize: 20, weight: .medium),
    .foregroundColor: NSColor.white,
]

for (index, item) in loadedItems.enumerated() {
    let column = index % columnCount
    let rowFromTop = index / columnCount
    let cellX = CGFloat(column) * cellWidth
    let cellY = CGFloat(rowCount - rowFromTop - 1) * cellHeight
    item.image.draw(
        in: NSRect(x: cellX, y: cellY + labelHeight, width: cellWidth, height: sourceSize.height),
        from: .zero,
        operation: .copy,
        fraction: 1.0)
    (item.label as NSString).draw(
        at: NSPoint(x: cellX + 16, y: cellY + 14),
        withAttributes: labelStyle)
}

canvas.unlockFocus()
guard let tiff = canvas.tiffRepresentation,
      let bitmap = NSBitmapImageRep(data: tiff),
      let png = bitmap.representation(using: .png, properties: [:]) else {
    fail("could not encode the contact sheet")
}

do {
    try png.write(to: URL(fileURLWithPath: outputPath), options: .atomic)
} catch {
    fail("could not write '\(outputPath)': \(error.localizedDescription)")
}

print("Wrote \(outputPath) (\(bitmap.pixelsWide)x\(bitmap.pixelsHigh), \(loadedItems.count) items)")

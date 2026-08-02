using System.Reflection;
using System.Runtime.InteropServices;
using ChronoFall.CharacterExperiment.SdlGpu;
using ChronoFall.CharacterExperiment.SimpleMesh;
using ChronoFall.CharacterPresentation.Cooking;
using SDL;

namespace ChronoFall.CharacterExperiment.GpuHarness;

public static class Program
{
    private const string SelectedAsset = "assets/Quaternius/Universal Animation Library[Standard]/Unreal-Godot/UAL1_Standard.glb";
    private const string SelectedAnimation = "Walk_Loop";

    public static int Main(string[] args)
    {
        Console.WriteLine("GPU_HARNESS_START");
        try
        {
            ConfigureNativeSdl();
            HarnessArguments options = HarnessArguments.Parse(args);
            if (options.StaticProof)
            {
                StaticMeshDefinition? mesh = LoadStaticAsset(options.CookedStaticAssetPath);
                StaticMeshHarnessResult staticResult = SdlGpuStaticMeshHarness.Run(
                    new StaticMeshHarnessOptions(
                        512,
                        512,
                        options.Visible,
                        options.StaticCapturePath,
                        mesh));
                Console.WriteLine(
                    $"GPU_STATIC_HARNESS_SUCCESS shader={staticResult.ShaderFormat} " +
                    $"baseline={staticResult.BaselineFingerprint:x16} " +
                    $"transformed={staticResult.TransformedFingerprint:x16}");
                return 0;
            }
            SkeletalCharacterAsset asset = LoadAsset(options);
            AnimationClip animation = SelectAnimation(asset);
            CharacterHarnessResult result = SdlGpuCharacterHarness.Run(
                asset,
                animation,
                new CharacterHarnessOptions(
                    512,
                    512,
                    options.Visible,
                    options.CapturePath,
                    options.SkeletonCapturePath,
                    options.AnimationCapturePath,
                    options.CaptureSuiteDirectory,
                    options.BlendCaptureSuiteDirectory,
                    options.LayeredCaptureSuiteDirectory,
                    options.IkAimCaptureSuiteDirectory));
            Console.WriteLine(
                $"GPU_HARNESS_SUCCESS shader={result.ShaderFormat} bind={result.BindPoseFingerprint:x16} " +
                $"probe={result.TranslatedProbeFingerprint:x16} skeleton={result.SkeletonDebugFingerprint:x16} " +
                $"animation={result.AnimationSampleFingerprint:x16} " +
                $"blend={result.Blend.LocomotionMidpointFingerprint:x16}/" +
                $"{result.Blend.ActionBodyFingerprint:x16} layer={result.Layered.UpperBodyActionFingerprint:x16} " +
                $"ik-aim={result.IkAim.CombinedFingerprint:x16}");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"GPU_HARNESS_FAILURE: {exception}");
            return 1;
        }
    }

    private static SkeletalCharacterAsset LoadAsset(HarnessArguments options)
    {
        if (options.AssetPath is not null && options.CookedAssetPath is not null)
            throw new ArgumentException("--asset and --cooked-asset cannot be combined.");

        if (options.CookedAssetPath is not null)
        {
            using FileStream stream = File.OpenRead(options.CookedAssetPath);
            CookedSkeletalCharacterAsset cooked = SkeletalAssetCookedFormat.Read(stream);
            Console.WriteLine(
                $"GPU_HARNESS_ASSET cooked id={cooked.Descriptor.AssetId} " +
                $"source={cooked.Descriptor.SourcePath} clips={cooked.Asset.Animations.Count}");
            return cooked.Asset;
        }

        string assetPath = options.AssetPath ?? Path.Combine(FindRepositoryRoot(), SelectedAsset);
        Console.WriteLine($"GPU_HARNESS_ASSET source path={assetPath}");
        return SimpleMeshSkeletalAssetLoader.LoadFromFile(assetPath);
    }

    private static StaticMeshDefinition? LoadStaticAsset(string? path)
    {
        if (path is null)
            return null;
        using FileStream stream = File.OpenRead(path);
        CookedStaticMeshAsset cooked = StaticMeshCookedFormat.Read(stream);
        Console.WriteLine(
            $"GPU_STATIC_HARNESS_ASSET cooked id={cooked.Descriptor.AssetId} " +
            $"source={cooked.Descriptor.PrimarySource.Path} sections={cooked.Mesh.Sections.Count}");
        return cooked.Mesh;
    }

    private static AnimationClip SelectAnimation(SkeletalCharacterAsset asset)
    {
        AnimationClip? selected = asset.Animations.SingleOrDefault(
            candidate => string.Equals(candidate.Name, SelectedAnimation, StringComparison.Ordinal));
        if (selected is not null)
            return selected;

        string available = string.Join(", ", asset.Animations.Select(static candidate => candidate.Name));
        throw new InvalidOperationException(
            $"Required animation '{SelectedAnimation}' was not found by ordinal name. Available clips: {available}");
    }

    private static void ConfigureNativeSdl()
    {
        NativeLibrary.SetDllImportResolver(typeof(SDL3).Assembly, ResolveNativeLibrary);
    }

    private static IntPtr ResolveNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, "SDL3", StringComparison.Ordinal))
            return IntPtr.Zero;
        if (!OperatingSystem.IsMacOS() || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            return IntPtr.Zero;

        string path = Path.Combine(AppContext.BaseDirectory, "runtimes", "osx-arm64", "native", "libSDL3.dylib");
        if (!File.Exists(path))
            throw new DllNotFoundException($"SDL3 was not bundled for osx-arm64. Expected path: {path}");
        return NativeLibrary.Load(path);
    }

    private static string FindRepositoryRoot()
    {
        foreach (string start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "ChronoFall.slnx")))
                    return directory.FullName;
                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("Could not find the ChronoFall repository root containing ChronoFall.slnx.");
    }

    private sealed record HarnessArguments(
        bool Visible,
        bool StaticProof,
        string? AssetPath,
        string? CookedAssetPath,
        string? CookedStaticAssetPath,
        string? StaticCapturePath,
        string? CapturePath,
        string? SkeletonCapturePath,
        string? AnimationCapturePath,
        string? CaptureSuiteDirectory,
        string? BlendCaptureSuiteDirectory,
        string? LayeredCaptureSuiteDirectory,
        string? IkAimCaptureSuiteDirectory)
    {
        internal static HarnessArguments Parse(string[] args)
        {
            bool visible = false;
            bool staticProof = false;
            string? asset = null;
            string? cookedAsset = null;
            string? cookedStaticAsset = null;
            string? staticCapture = null;
            string? capture = null;
            string? skeletonCapture = null;
            string? animationCapture = null;
            string? captureSuite = null;
            string? blendCaptureSuite = null;
            string? layeredCaptureSuite = null;
            string? ikAimCaptureSuite = null;
            for (int index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--visible":
                        visible = true;
                        break;
                    case "--static-proof":
                        staticProof = true;
                        break;
                    case "--static-capture" when index + 1 < args.Length:
                        staticCapture = Path.GetFullPath(args[++index]);
                        break;
                    case "--asset" when index + 1 < args.Length:
                        asset = Path.GetFullPath(args[++index]);
                        break;
                    case "--cooked-asset" when index + 1 < args.Length:
                        cookedAsset = Path.GetFullPath(args[++index]);
                        break;
                    case "--cooked-static-asset" when index + 1 < args.Length:
                        cookedStaticAsset = Path.GetFullPath(args[++index]);
                        break;
                    case "--capture" when index + 1 < args.Length:
                        capture = Path.GetFullPath(args[++index]);
                        break;
                    case "--skeleton-capture" when index + 1 < args.Length:
                        skeletonCapture = Path.GetFullPath(args[++index]);
                        break;
                    case "--animation-capture" when index + 1 < args.Length:
                        animationCapture = Path.GetFullPath(args[++index]);
                        break;
                    case "--capture-suite" when index + 1 < args.Length:
                        captureSuite = Path.GetFullPath(args[++index]);
                        break;
                    case "--blend-capture-suite" when index + 1 < args.Length:
                        blendCaptureSuite = Path.GetFullPath(args[++index]);
                        break;
                    case "--layered-capture-suite" when index + 1 < args.Length:
                        layeredCaptureSuite = Path.GetFullPath(args[++index]);
                        break;
                    case "--ik-aim-capture-suite" when index + 1 < args.Length:
                        ikAimCaptureSuite = Path.GetFullPath(args[++index]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown or incomplete GPU harness argument '{args[index]}'.");
                }
            }
            if (staticProof)
            {
                if (asset is not null || cookedAsset is not null || capture is not null ||
                    skeletonCapture is not null || animationCapture is not null ||
                    captureSuite is not null || blendCaptureSuite is not null ||
                    layeredCaptureSuite is not null || ikAimCaptureSuite is not null)
                {
                    throw new ArgumentException("--static-proof cannot be combined with character asset or capture arguments.");
                }
            }
            else if (staticCapture is not null)
            {
                throw new ArgumentException("--static-capture requires --static-proof.");
            }
            else if (cookedStaticAsset is not null)
            {
                throw new ArgumentException("--cooked-static-asset requires --static-proof.");
            }
            return new HarnessArguments(
                visible,
                staticProof,
                asset,
                cookedAsset,
                cookedStaticAsset,
                staticCapture,
                capture,
                skeletonCapture,
                animationCapture,
                captureSuite,
                blendCaptureSuite,
                layeredCaptureSuite,
                ikAimCaptureSuite);
        }
    }
}

using System.Reflection;
using System.Runtime.InteropServices;
using ChronoFall.CharacterExperiment.SdlGpu;
using ChronoFall.CharacterExperiment.SimpleMesh;
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
            string assetPath = options.AssetPath ?? Path.Combine(FindRepositoryRoot(), SelectedAsset);
            SkeletalCharacterAsset asset = SimpleMeshSkeletalAssetLoader.LoadFromFile(assetPath);
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
                    options.CaptureSuiteDirectory));
            Console.WriteLine(
                $"GPU_HARNESS_SUCCESS shader={result.ShaderFormat} bind={result.BindPoseFingerprint:x16} " +
                $"probe={result.TranslatedProbeFingerprint:x16} skeleton={result.SkeletonDebugFingerprint:x16} " +
                $"animation={result.AnimationSampleFingerprint:x16}");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"GPU_HARNESS_FAILURE: {exception}");
            return 1;
        }
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
        string? AssetPath,
        string? CapturePath,
        string? SkeletonCapturePath,
        string? AnimationCapturePath,
        string? CaptureSuiteDirectory)
    {
        internal static HarnessArguments Parse(string[] args)
        {
            bool visible = false;
            string? asset = null;
            string? capture = null;
            string? skeletonCapture = null;
            string? animationCapture = null;
            string? captureSuite = null;
            for (int index = 0; index < args.Length; index++)
            {
                switch (args[index])
                {
                    case "--visible":
                        visible = true;
                        break;
                    case "--asset" when index + 1 < args.Length:
                        asset = Path.GetFullPath(args[++index]);
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
                    default:
                        throw new ArgumentException($"Unknown or incomplete GPU harness argument '{args[index]}'.");
                }
            }
            return new HarnessArguments(visible, asset, capture, skeletonCapture, animationCapture, captureSuite);
        }
    }
}

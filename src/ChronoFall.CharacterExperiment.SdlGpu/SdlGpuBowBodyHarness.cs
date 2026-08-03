using System.Globalization;
using System.Numerics;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record BowBodyHarnessOptions(
    int CaptureWidth = 512,
    int CaptureHeight = 512,
    bool Visible = false,
    int VisibleWidth = 1920,
    int VisibleHeight = 1080,
    string? CaptureSuiteDirectory = null,
    string? ReleaseFrameDirectory = null);

internal sealed record BowBodyCapture(
    string Name,
    float SequenceTime,
    string Clip,
    float SampleTime,
    int SampleFrame,
    ulong Fingerprint);

internal sealed record BowBodyHarnessResult(
    SDL_GPUShaderFormat ShaderFormat,
    IReadOnlyList<BowBodyCapture> Captures,
    int ShootFrameCount,
    int RapidShootFrameCount);

internal static partial class SdlGpuCharacterHarness
{
    internal static BowBodyHarnessResult RunBowBodyProof(
        SkeletalCharacterAsset asset,
        AnimationClip referenceIdle,
        BowBodyHarnessOptions options)
    {
        ArgumentNullException.ThrowIfNull(asset);
        ArgumentNullException.ThrowIfNull(referenceIdle);
        ArgumentNullException.ThrowIfNull(options);
        if (options.CaptureWidth < 64 || options.CaptureHeight < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "Bow-body captures must be at least 64x64.");
        if (options.VisibleWidth < 64 || options.VisibleHeight < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "The bow-body review window must be at least 64x64.");

        AnimationClip walk = SelectAnimation(asset, "Walk_Fwd_Loop");
        AnimationClip notch = SelectAnimation(asset, "Bow_Notch");
        AnimationClip aimNeutral = SelectAnimation(asset, "Bow_Aim_Neutral");
        AnimationClip shoot = SelectAnimation(asset, "Bow_Shoot");
        AnimationClip aimUp = SelectAnimation(asset, "Bow_Aim_Up");
        AnimationClip rapidShoot = SelectAnimation(asset, "Bow_RapidShoot_Loop");
        if (!ReferenceEquals(referenceIdle.Skeleton, asset.Mesh.Skin.Skeleton))
            throw new ArgumentException("The reference idle must already be rebound to the cooked UAL2 skeleton.", nameof(referenceIdle));

        var playback = new BowBodyPlaybackController(
            referenceIdle,
            walk,
            notch,
            aimNeutral,
            shoot,
            aimUp,
            rapidShoot);
        var referenceSequence = new BowBodySequence(
            referenceIdle,
            walk,
            notch,
            aimNeutral,
            shoot,
            aimUp,
            rapidShoot);

        MeshBounds bounds = MeshBounds.Create(asset.Mesh.Vertices.Select(static vertex => vertex.Position).ToArray());
        SkeletonPose bindPose = asset.Mesh.Skin.Skeleton.CreateBindPose();
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(bindPose);
        float skeletonAxisLength = bounds.Radius * 0.04f;
        SkeletonDebugGeometry skeletonDebug = SkeletonDebugGeometry.Create(globalPose, skeletonAxisLength);
        BindPoseCamera captureCamera = BindPoseCamera.Create(bounds, options.CaptureWidth, options.CaptureHeight);

        var captures = new List<BowBodyCapture>();
        SDL_GPUShaderFormat shaderFormat;
        using (var gpu = new CharacterGpuSession(
            asset.Mesh,
            skeletonDebug,
            options.CaptureWidth,
            options.CaptureHeight,
            visible: false))
        {
            shaderFormat = gpu.ShaderFormat;
            CaptureClip(
                gpu,
                asset.Mesh.Skin,
                referenceIdle,
                0.5f,
                AnimationPlaybackMode.Loop,
                captureCamera.ViewProjection,
                options,
                captures,
                "idle-ual1-loop");
            CaptureSequenceStages(
                gpu,
                asset.Mesh.Skin,
                referenceSequence,
                captureCamera.ViewProjection,
                options,
                captures);
            CaptureReleaseFrames(
                gpu,
                asset.Mesh.Skin,
                shoot,
                rapidShoot,
                captureCamera.ViewProjection,
                options);
        }

        int shootFrameCount = BowBodySequence.ToFrame(shoot.Duration, shoot.Duration) + 1;
        int rapidFrameCount = BowBodySequence.ToFrame(rapidShoot.Duration, rapidShoot.Duration) + 1;
        Console.WriteLine(
            $"GPU_BOW_BODY_PASS captures={captures.Count} shoot-frames={shootFrameCount} " +
            $"rapid-frames={rapidFrameCount} shader={shaderFormat}");

        if (options.Visible)
        {
            BindPoseCamera visibleCamera = BindPoseCamera.Create(bounds, options.VisibleWidth, options.VisibleHeight);
            using var gpu = new CharacterGpuSession(
                asset.Mesh,
                skeletonDebug,
                options.VisibleWidth,
                options.VisibleHeight,
                visible: true);
            gpu.RunBowBodyVisible(
                visibleCamera.ViewProjection,
                playback,
                asset.Mesh.Skin,
                skeletonAxisLength);
        }

        return new BowBodyHarnessResult(
            shaderFormat,
            captures.AsReadOnly(),
            shootFrameCount,
            rapidFrameCount);
    }

    private static void CaptureSequenceStages(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        BowBodySequence sequence,
        Matrix4x4 viewProjection,
        BowBodyHarnessOptions options,
        List<BowBodyCapture> captures)
    {
        float start = 0.0f;
        foreach (BowBodySequenceSegment segment in sequence.Segments)
        {
            bool selected = segment.Kind is
                BowBodySegmentKind.Neutral or
                BowBodySegmentKind.Notch or
                BowBodySegmentKind.AimNeutral or
                BowBodySegmentKind.Shoot or
                BowBodySegmentKind.Recovery or
                BowBodySegmentKind.RepeatShoot or
                BowBodySegmentKind.Walk or
                BowBodySegmentKind.AimUp or
                BowBodySegmentKind.RapidShoot or
                BowBodySegmentKind.FinalRecovery;
            if (selected)
            {
                float sequenceTime = start + segment.Duration * 0.5f;
                BowBodyFrame frame = sequence.Evaluate(sequenceTime);
                byte[] pixels = RenderBowPose(gpu, skin, frame.Pose, viewProjection);
                string name = $"sequence-{captures.Count - 1:D2}-{segment.Kind.ToString().ToLowerInvariant()}";
                WriteOptionalCapture(options.CaptureSuiteDirectory, name, options.CaptureWidth, options.CaptureHeight, pixels);
                captures.Add(new BowBodyCapture(
                    name,
                    sequenceTime,
                    frame.Clip.Name,
                    frame.SampleTime,
                    frame.SampleFrame,
                    Fingerprint(pixels)));
            }
            start += segment.Duration;
        }
    }

    private static void CaptureClip(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        AnimationClip clip,
        float sampleTime,
        AnimationPlaybackMode playback,
        Matrix4x4 viewProjection,
        BowBodyHarnessOptions options,
        List<BowBodyCapture> captures,
        string name)
    {
        SkeletonPose pose = AnimationSampler.Sample(clip, sampleTime, playback);
        byte[] pixels = RenderBowPose(gpu, skin, pose, viewProjection);
        WriteOptionalCapture(options.CaptureSuiteDirectory, name, options.CaptureWidth, options.CaptureHeight, pixels);
        captures.Add(new BowBodyCapture(
            name,
            0.0f,
            clip.Name,
            sampleTime,
            BowBodySequence.ToFrame(sampleTime, clip.Duration),
            Fingerprint(pixels)));
    }

    private static void CaptureReleaseFrames(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        AnimationClip shoot,
        AnimationClip rapidShoot,
        Matrix4x4 viewProjection,
        BowBodyHarnessOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ReleaseFrameDirectory))
            return;
        string root = Path.GetFullPath(options.ReleaseFrameDirectory);
        Directory.CreateDirectory(root);
        CaptureEveryFrame(gpu, skin, shoot, viewProjection, root, "bow-shoot", options.CaptureWidth, options.CaptureHeight);
        CaptureEveryFrame(gpu, skin, rapidShoot, viewProjection, root, "bow-rapid", options.CaptureWidth, options.CaptureHeight);
    }

    private static void CaptureEveryFrame(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        AnimationClip clip,
        Matrix4x4 viewProjection,
        string root,
        string prefix,
        int width,
        int height)
    {
        int lastFrame = BowBodySequence.ToFrame(clip.Duration, clip.Duration);
        for (int frame = 0; frame <= lastFrame; frame++)
        {
            float sampleTime = Math.Min(frame / BowBodySequence.FrameRate, clip.Duration);
            SkeletonPose pose = AnimationSampler.Sample(clip, sampleTime, AnimationPlaybackMode.Clamp);
            byte[] pixels = RenderBowPose(gpu, skin, pose, viewProjection);
            WritePpm(Path.Combine(root, $"{prefix}-{frame:D2}-{sampleTime * 1000.0f:F0}ms.ppm"), width, height, pixels);
        }
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"GPU_BOW_BODY_RELEASE_FRAMES clip={clip.Name} frames={lastFrame + 1} directory={root}"));
    }

    private static byte[] RenderBowPose(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        SkeletonPose pose,
        Matrix4x4 viewProjection)
    {
        CharacterAnimationFrame frame = CreateAnimationFrame(skin, pose, 0.0f);
        gpu.UploadPalette(frame.Palette);
        return gpu.RenderOffscreen(viewProjection, includeSkeleton: false);
    }

    private static void WriteOptionalCapture(
        string? directory,
        string name,
        int width,
        int height,
        byte[] pixels)
    {
        if (string.IsNullOrWhiteSpace(directory))
            return;
        string fullDirectory = Path.GetFullPath(directory);
        Directory.CreateDirectory(fullDirectory);
        WritePpm(Path.Combine(fullDirectory, name + ".ppm"), width, height, pixels);
    }

    private sealed unsafe partial class CharacterGpuSession
    {
        internal void RunBowBodyVisible(
            Matrix4x4 viewProjection,
            BowBodyPlaybackController playback,
            SkinDefinition skin,
            float skeletonAxisLength)
        {
            ArgumentNullException.ThrowIfNull(playback);
            ArgumentNullException.ThrowIfNull(skin);
            if (!SDL_ShowWindow(window))
                throw new InvalidOperationException($"SDL could not show the bow-body review window: {SDL_GetError()}");

            ulong frequency = SDL_GetPerformanceFrequency();
            if (frequency == 0)
                throw new InvalidOperationException("SDL returned a zero performance-counter frequency.");
            ulong previousCounter = SDL_GetPerformanceCounter();
            ulong lastTitleCounter = 0;
            bool titleDirty = true;
            Console.WriteLine(
                "GPU_BOW_BODY_CONTROLS 1=full-sequence 2=Bow_Shoot-frames 3=RapidShoot-frames " +
                "Left/Right=step-frame Space=pause/resume R=restart D=skeleton Escape=close");
            Console.WriteLine(playback.CreateDiagnostic());

            bool running = true;
            while (running)
            {
                SDL_Event sdlEvent;
                while (SDL_PollEvent(&sdlEvent))
                {
                    if (sdlEvent.Type is SDL_EventType.SDL_EVENT_QUIT or SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED ||
                        sdlEvent.Type == SDL_EventType.SDL_EVENT_KEY_DOWN && sdlEvent.key.key == SDL_Keycode.SDLK_ESCAPE)
                    {
                        running = false;
                        continue;
                    }

                    if (sdlEvent.Type == SDL_EventType.SDL_EVENT_KEY_DOWN &&
                        ApplyBowBodyControl(playback, sdlEvent.key.key))
                    {
                        titleDirty = true;
                        Console.WriteLine(playback.CreateDiagnostic());
                    }
                }
                if (!running)
                    break;

                ulong currentCounter = SDL_GetPerformanceCounter();
                double elapsedSeconds = (currentCounter - previousCounter) / (double)frequency;
                previousCounter = currentCounter;
                playback.Advance(elapsedSeconds);
                BowBodyFrame bowFrame = playback.CreateFrame();
                ExecuteInteractiveFrame(bowFrame.Clip, bowFrame.SampleTime, jointCount, () =>
                {
                    CharacterAnimationFrame frame = CreateAnimationFrame(skin, bowFrame.Pose, bowFrame.SampleTime);
                    UploadPalette(frame.Palette);
                    if (playback.IsSkeletonVisible)
                    {
                        SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, skeletonAxisLength);
                        UploadSkeleton(skeleton.Vertices);
                    }
                    if (titleDirty || currentCounter - lastTitleCounter >= frequency / 10)
                    {
                        SetWindowTitle(playback.CreateDiagnostic());
                        lastTitleCounter = currentCounter;
                        titleDirty = false;
                    }

                    SDL_GPUCommandBuffer* command = AcquireCommand();
                    SDL_GPUTexture* swapchain;
                    uint swapchainWidth;
                    uint swapchainHeight;
                    if (!SDL_WaitAndAcquireGPUSwapchainTexture(command, window, &swapchain, &swapchainWidth, &swapchainHeight))
                        throw new InvalidOperationException($"SDL GPU bow-body swapchain acquisition failed: {SDL_GetError()}");
                    if (swapchain is not null)
                    {
                        EnsureVisibleDepth(swapchainWidth, swapchainHeight);
                        Render(
                            command,
                            swapchain,
                            visibleDepth,
                            swapchainWidth,
                            swapchainHeight,
                            viewProjection,
                            includeSkeleton: playback.IsSkeletonVisible);
                    }
                    if (!SDL_SubmitGPUCommandBuffer(command))
                        throw new InvalidOperationException($"SDL GPU bow-body submission failed: {SDL_GetError()}");
                });
                SDL_Delay(16);
            }
        }

        private static bool ApplyBowBodyControl(BowBodyPlaybackController playback, SDL_Keycode key)
        {
            switch (key)
            {
                case SDL_Keycode.SDLK_1:
                    playback.SelectMode(BowBodyViewMode.FullSequence);
                    return true;
                case SDL_Keycode.SDLK_2:
                    playback.SelectMode(BowBodyViewMode.ShootFrames);
                    return true;
                case SDL_Keycode.SDLK_3:
                    playback.SelectMode(BowBodyViewMode.RapidShootFrames);
                    return true;
                case SDL_Keycode.SDLK_LEFT:
                    playback.StepFrames(-1);
                    return true;
                case SDL_Keycode.SDLK_RIGHT:
                    playback.StepFrames(1);
                    return true;
                case SDL_Keycode.SDLK_SPACE:
                    playback.TogglePlaying();
                    return true;
                case SDL_Keycode.SDLK_R:
                    playback.Restart();
                    return true;
                case SDL_Keycode.SDLK_D:
                    playback.ToggleSkeleton();
                    return true;
                default:
                    return false;
            }
        }
    }
}

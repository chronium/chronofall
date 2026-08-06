using System.Globalization;
using System.Numerics;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record SocketedBowHarnessOptions(
    int CaptureWidth = 512,
    int CaptureHeight = 512,
    bool Visible = false,
    int VisibleWidth = 1920,
    int VisibleHeight = 1080,
    string? CaptureSuiteDirectory = null);

internal sealed record SocketedBowHarnessResult(
    SDL_GPUShaderFormat ShaderFormat,
    int JointIndex,
    int FirstBowPixels,
    int SecondBowPixels,
    ulong FirstFingerprint,
    ulong SecondFingerprint,
    ulong RepeatedFingerprint,
    Matrix4x4 FirstBowWorld,
    Matrix4x4 SecondBowWorld);

internal static partial class SdlGpuCharacterHarness
{
    private const float SocketedBowSecondSampleTime = 0.5f;

    internal static SocketedBowHarnessResult RunSocketedBowProof(
        SkeletalCharacterAsset asset,
        StaticMeshDefinition bow,
        SocketedBowHarnessOptions options)
    {
        ArgumentNullException.ThrowIfNull(asset);
        ArgumentNullException.ThrowIfNull(bow);
        ArgumentNullException.ThrowIfNull(options);
        if (options.CaptureWidth < 64 || options.CaptureHeight < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "Socketed-bow captures must be at least 64x64.");
        if (options.VisibleWidth < 64 || options.VisibleHeight < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "The socketed-bow review window must be at least 64x64.");

        AnimationClip idle = SelectAnimation(asset, "Idle_Loop");
        var attachment = new TechnicalSocketedBowAttachment(
            asset.Mesh.Skin.Skeleton,
            TechnicalSocketedBowAttachment.DefaultBowLocalTransform);
        CharacterAnimationFrame firstFrame = CreateAnimationFrame(asset.Mesh.Skin, idle, 0.0f);
        CharacterAnimationFrame secondFrame = CreateAnimationFrame(
            asset.Mesh.Skin,
            idle,
            SocketedBowSecondSampleTime);
        TechnicalSocketedBowFrame firstAttachment = attachment.Evaluate(firstFrame.GlobalPose, Matrix4x4.Identity);
        TechnicalSocketedBowFrame secondAttachment = attachment.Evaluate(secondFrame.GlobalPose, Matrix4x4.Identity);
        Require(
            firstAttachment.BowWorldTransform != secondAttachment.BowWorldTransform,
            "The selected Idle_Loop samples did not move the technical bow transform.");

        MeshBounds combinedBounds = CreateSocketedBowBounds(
            asset.Mesh,
            bow,
            firstAttachment.BowWorldTransform,
            secondAttachment.BowWorldTransform);
        BindPoseCamera captureCamera = BindPoseCamera.Create(
            combinedBounds,
            options.CaptureWidth,
            options.CaptureHeight);
        SkeletonPose bindPose = asset.Mesh.Skin.Skeleton.CreateBindPose();
        SkeletonGlobalPose bindGlobal = SkeletonPoseEvaluator.EvaluateGlobal(bindPose);
        float skeletonAxisLength = combinedBounds.Radius * 0.04f;
        SkeletonDebugGeometry skeletonDebug = SkeletonDebugGeometry.Create(bindGlobal, skeletonAxisLength);

        byte[] firstPixels;
        byte[] secondPixels;
        byte[] repeatedPixels;
        SDL_GPUShaderFormat shaderFormat;
        using (var gpu = new CharacterGpuSession(
            asset.Mesh,
            skeletonDebug,
            options.CaptureWidth,
            options.CaptureHeight,
            visible: false,
            attachmentSource: bow))
        {
            shaderFormat = gpu.ShaderFormat;
            firstPixels = RenderSocketedBowFrame(
                gpu,
                firstFrame,
                firstAttachment.BowWorldTransform,
                captureCamera.ViewProjection);
            secondPixels = RenderSocketedBowFrame(
                gpu,
                secondFrame,
                secondAttachment.BowWorldTransform,
                captureCamera.ViewProjection);
            repeatedPixels = RenderSocketedBowFrame(
                gpu,
                firstFrame,
                firstAttachment.BowWorldTransform,
                captureCamera.ViewProjection);
        }

        int firstBowPixels = CountTechnicalBowPixels(firstPixels);
        int secondBowPixels = CountTechnicalBowPixels(secondPixels);
        ulong firstFingerprint = Fingerprint(firstPixels);
        ulong secondFingerprint = Fingerprint(secondPixels);
        ulong repeatedFingerprint = Fingerprint(repeatedPixels);
        Require(firstBowPixels >= 100, $"The first socketed-bow capture contained only {firstBowPixels} bow pixels.");
        Require(secondBowPixels >= 100, $"The second socketed-bow capture contained only {secondBowPixels} bow pixels.");
        Require(firstFingerprint != secondFingerprint, "Distinct Idle_Loop samples produced the same socketed-bow capture.");
        Require(firstFingerprint == repeatedFingerprint, "The repeated socketed-bow capture was not byte-identical.");

        if (!string.IsNullOrWhiteSpace(options.CaptureSuiteDirectory))
        {
            string directory = Path.GetFullPath(options.CaptureSuiteDirectory);
            WritePpm(Path.Combine(directory, "socketed-bow-idle-0000ms.ppm"), options.CaptureWidth, options.CaptureHeight, firstPixels);
            WritePpm(Path.Combine(directory, "socketed-bow-idle-0500ms.ppm"), options.CaptureWidth, options.CaptureHeight, secondPixels);
            Console.WriteLine($"GPU_SOCKETED_BOW_CAPTURE_SUITE {directory}");
        }

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"GPU_SOCKETED_BOW_PASS joint={TechnicalSocketedBowAttachment.JointName}:{attachment.JointIndex} " +
            $"clip={idle.Name} samples=0.000/{SocketedBowSecondSampleTime:F3} " +
            $"bow-pixels={firstBowPixels}/{secondBowPixels} " +
            $"fingerprints={firstFingerprint:x16}/{secondFingerprint:x16}/{repeatedFingerprint:x16} " +
            $"shader={shaderFormat}"));

        if (options.Visible)
        {
            BindPoseCamera visibleCamera = BindPoseCamera.Create(
                combinedBounds,
                options.VisibleWidth,
                options.VisibleHeight);
            using var gpu = new CharacterGpuSession(
                asset.Mesh,
                skeletonDebug,
                options.VisibleWidth,
                options.VisibleHeight,
                visible: true,
                attachmentSource: bow);
            gpu.RunSocketedBowVisible(
                visibleCamera.ViewProjection,
                idle,
                asset.Mesh.Skin,
                attachment,
                skeletonAxisLength);
        }

        return new SocketedBowHarnessResult(
            shaderFormat,
            attachment.JointIndex,
            firstBowPixels,
            secondBowPixels,
            firstFingerprint,
            secondFingerprint,
            repeatedFingerprint,
            firstAttachment.BowWorldTransform,
            secondAttachment.BowWorldTransform);
    }

    private static byte[] RenderSocketedBowFrame(
        CharacterGpuSession gpu,
        CharacterAnimationFrame frame,
        Matrix4x4 bowWorld,
        Matrix4x4 viewProjection)
    {
        gpu.UploadPalette(frame.Palette);
        return gpu.RenderOffscreen(viewProjection, includeSkeleton: false, bowWorld);
    }

    private static MeshBounds CreateSocketedBowBounds(
        SkinnedMeshDefinition character,
        StaticMeshDefinition bow,
        Matrix4x4 firstBowWorld,
        Matrix4x4 secondBowWorld)
    {
        var positions = new List<Vector3>(checked(character.Vertices.Count + bow.Vertices.Count * 2));
        positions.AddRange(character.Vertices.Select(static vertex => vertex.Position));
        positions.AddRange(bow.Vertices.Select(vertex => Vector3.Transform(vertex.Position, firstBowWorld)));
        positions.AddRange(bow.Vertices.Select(vertex => Vector3.Transform(vertex.Position, secondBowWorld)));
        return MeshBounds.Create(positions);
    }

    private static int CountTechnicalBowPixels(ReadOnlySpan<byte> pixels)
    {
        int count = 0;
        for (int offset = 0; offset < pixels.Length; offset += 4)
        {
            byte red = pixels[offset];
            byte green = pixels[offset + 1];
            byte blue = pixels[offset + 2];
            if (red >= 70 && green >= 45 && red > green && green > blue * 1.5f)
                count++;
        }
        return count;
    }

    private sealed unsafe partial class CharacterGpuSession
    {
        internal void RunSocketedBowVisible(
            Matrix4x4 viewProjection,
            AnimationClip idle,
            SkinDefinition skin,
            TechnicalSocketedBowAttachment attachment,
            float skeletonAxisLength)
        {
            ArgumentNullException.ThrowIfNull(idle);
            ArgumentNullException.ThrowIfNull(skin);
            ArgumentNullException.ThrowIfNull(attachment);
            if (!ReferenceEquals(idle.Skeleton, skin.Skeleton))
                throw new ArgumentException("The socketed-bow idle clip must use the character skin skeleton.", nameof(idle));
            if (!SDL_ShowWindow(window))
                throw new InvalidOperationException($"SDL could not show the socketed-bow review window: {SDL_GetError()}");

            ulong frequency = SDL_GetPerformanceFrequency();
            if (frequency == 0)
                throw new InvalidOperationException("SDL returned a zero performance-counter frequency.");
            ulong previousCounter = SDL_GetPerformanceCounter();
            float sampleTime = 0.0f;
            bool playing = true;
            bool skeletonVisible = false;
            bool turntableDragging = false;
            float turntableYaw = 0.0f;
            float gripOffsetMetres = TechnicalSocketedBowAttachment.DefaultGripOffsetMetres;
            float palmDepthMetres = TechnicalSocketedBowAttachment.DefaultPalmDepthMetres;
            float twistDegrees = TechnicalSocketedBowAttachment.DefaultTwistDegrees;
            TechnicalSocketedBowAttachment reviewAttachment = attachment;
            Console.WriteLine(
                "GPU_SOCKETED_BOW_CONTROLS Left-drag=rotate-character Space=pause/resume " +
                "R=restart C=reset-rotation D=skeleton [ ]=twist-5/+5deg " +
                ", .=grip-in/out-1cm ; '=palm-depth-1/+1cm Escape=close");

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
                    if (sdlEvent.Type != SDL_EventType.SDL_EVENT_KEY_DOWN || sdlEvent.key.repeat)
                    {
                        if (sdlEvent.Type == SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN &&
                            sdlEvent.button.button == (byte)SDLButton.SDL_BUTTON_LEFT)
                        {
                            turntableDragging = true;
                        }
                        else if (sdlEvent.Type == SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP &&
                            sdlEvent.button.button == (byte)SDLButton.SDL_BUTTON_LEFT)
                        {
                            turntableDragging = false;
                        }
                        else if (sdlEvent.Type == SDL_EventType.SDL_EVENT_MOUSE_MOTION && turntableDragging)
                        {
                            turntableYaw = MathF.IEEERemainder(
                                turntableYaw + sdlEvent.motion.xrel * 0.01f,
                                MathF.Tau);
                        }
                        continue;
                    }
                    switch (sdlEvent.key.key)
                    {
                        case SDL_Keycode.SDLK_SPACE:
                            playing = !playing;
                            break;
                        case SDL_Keycode.SDLK_R:
                            sampleTime = 0.0f;
                            break;
                        case SDL_Keycode.SDLK_C:
                            turntableYaw = 0.0f;
                            break;
                        case SDL_Keycode.SDLK_D:
                            skeletonVisible = !skeletonVisible;
                            break;
                        case SDL_Keycode.SDLK_LEFTBRACKET:
                            twistDegrees -= 5.0f;
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                        case SDL_Keycode.SDLK_RIGHTBRACKET:
                            twistDegrees += 5.0f;
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                        case SDL_Keycode.SDLK_COMMA:
                            gripOffsetMetres = MathF.Max(0.0f, gripOffsetMetres - 0.01f);
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                        case SDL_Keycode.SDLK_PERIOD:
                            gripOffsetMetres = MathF.Min(0.12f, gripOffsetMetres + 0.01f);
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                        case SDL_Keycode.SDLK_SEMICOLON:
                            palmDepthMetres = MathF.Max(-0.05f, palmDepthMetres - 0.01f);
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                        case SDL_Keycode.SDLK_APOSTROPHE:
                            palmDepthMetres = MathF.Min(0.05f, palmDepthMetres + 0.01f);
                            reviewAttachment = CreateReviewAttachment(
                                skin, gripOffsetMetres, palmDepthMetres, twistDegrees);
                            break;
                    }
                }
                if (!running)
                    break;

                ulong currentCounter = SDL_GetPerformanceCounter();
                if (playing)
                {
                    sampleTime = AnimationSampler.ResolveTime(
                        idle,
                        sampleTime + (float)((currentCounter - previousCounter) / (double)frequency),
                        AnimationPlaybackMode.Loop);
                }
                previousCounter = currentCounter;

                ExecuteInteractiveFrame(idle, sampleTime, jointCount, () =>
                {
                    CharacterAnimationFrame frame = CreateAnimationFrame(skin, idle, sampleTime);
                    Matrix4x4 characterWorld = Matrix4x4.CreateRotationY(turntableYaw);
                    TechnicalSocketedBowFrame bowFrame = reviewAttachment.Evaluate(frame.GlobalPose, characterWorld);
                    UploadPalette(frame.Palette);
                    if (skeletonVisible)
                    {
                        SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, skeletonAxisLength);
                        UploadSkeleton(skeleton.Vertices);
                    }
                    SetWindowTitle(string.Create(
                        CultureInfo.InvariantCulture,
                        $"ChronoFall socketed bow proof | {idle.Name} {sampleTime:F3}s | " +
                        $"{TechnicalSocketedBowAttachment.JointName} | yaw {turntableYaw * 180.0f / MathF.PI:F1} deg | " +
                        $"twist {twistDegrees:F0} deg | grip {gripOffsetMetres:F2} m | " +
                        $"palm {palmDepthMetres:+0.00;-0.00;0.00} m"));

                    SDL_GPUCommandBuffer* command = AcquireCommand();
                    bool swapchainAcquired = false;
                    try
                    {
                        SDL_GPUTexture* swapchain;
                        uint swapchainWidth;
                        uint swapchainHeight;
                        if (!SDL_WaitAndAcquireGPUSwapchainTexture(command, window, &swapchain, &swapchainWidth, &swapchainHeight))
                            throw new InvalidOperationException($"SDL GPU socketed-bow swapchain acquisition failed: {SDL_GetError()}");
                        swapchainAcquired = true;
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
                                skeletonVisible,
                                bowFrame.BowWorldTransform,
                                characterWorld);
                        }
                        if (!SDL_SubmitGPUCommandBuffer(command))
                            throw new InvalidOperationException($"SDL GPU socketed-bow submission failed: {SDL_GetError()}");
                        command = null;
                    }
                    catch
                    {
                        if (command is not null && swapchainAcquired)
                        {
                            _ = SDL_SubmitGPUCommandBuffer(command);
                            command = null;
                        }
                        throw;
                    }
                    finally
                    {
                        if (command is not null)
                            _ = SDL_CancelGPUCommandBuffer(command);
                    }
                });
                SDL_Delay(16);
            }

            static TechnicalSocketedBowAttachment CreateReviewAttachment(
                SkinDefinition skin,
                float gripOffsetMetres,
                float palmDepthMetres,
                float twistDegrees) =>
                new(
                    skin.Skeleton,
                    TechnicalSocketedBowAttachment.CreateBowLocalTransform(
                        gripOffsetMetres,
                        palmDepthMetres,
                        twistDegrees,
                        TechnicalSocketedBowAttachment.DefaultRollDegrees));
        }
    }
}

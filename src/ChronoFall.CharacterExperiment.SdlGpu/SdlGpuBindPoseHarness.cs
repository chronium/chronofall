using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using SDL;
using static SDL.SDL3;

namespace ChronoFall.CharacterExperiment.SdlGpu;

internal sealed record CharacterHarnessOptions(
    int Width = 512,
    int Height = 512,
    bool Visible = false,
    string? CapturePath = null,
    string? SkeletonCapturePath = null,
    string? AnimationCapturePath = null,
    string? CaptureSuiteDirectory = null,
    string? BlendCaptureSuiteDirectory = null,
    string? LayeredCaptureSuiteDirectory = null,
    string? IkAimCaptureSuiteDirectory = null);

internal sealed record CharacterHarnessResult(
    SDL_GPUShaderFormat ShaderFormat,
    FrameAnalysis BindPose,
    FrameAnalysis TranslatedProbe,
    FrameAnalysis SkeletonDebug,
    FrameAnalysis AnimationStart,
    FrameAnalysis AnimationSample,
    FrameAnalysis AnimationLaterSample,
    FrameAnalysis AnimationLoopBoundary,
    SkeletonOverlayAnalysis SkeletonOverlay,
    ulong BindPoseFingerprint,
    ulong TranslatedProbeFingerprint,
    ulong SkeletonDebugFingerprint,
    ulong AnimationStartFingerprint,
    ulong AnimationSampleFingerprint,
    ulong AnimationLaterSampleFingerprint,
    ulong AnimationLoopBoundaryFingerprint,
    BlendHarnessResult Blend,
    LayeredHarnessResult Layered,
    IkAimHarnessResult IkAim,
    int SkeletonLineCount);

internal sealed record BlendHarnessResult(
    FrameAnalysis LocomotionIdle,
    FrameAnalysis LocomotionMidpoint,
    FrameAnalysis LocomotionWalk,
    FrameAnalysis ActionEntry,
    FrameAnalysis ActionBody,
    FrameAnalysis ActionReturn,
    ulong LocomotionIdleFingerprint,
    ulong LocomotionMidpointFingerprint,
    ulong LocomotionWalkFingerprint,
    ulong ActionEntryFingerprint,
    ulong ActionBodyFingerprint,
    ulong ActionReturnFingerprint);

internal sealed record LayeredHarnessResult(
    FrameAnalysis WalkBase,
    FrameAnalysis FullBodyAction,
    FrameAnalysis UpperBodyAction,
    FrameAnalysis ActionEntry,
    FrameAnalysis ActionReturn,
    FrameAnalysis WalkAdvanced,
    ulong WalkBaseFingerprint,
    ulong FullBodyActionFingerprint,
    ulong UpperBodyActionFingerprint,
    ulong ActionEntryFingerprint,
    ulong ActionReturnFingerprint,
    ulong WalkAdvancedFingerprint,
    int MaskRootIndex,
    int MaskedJointCount);

internal sealed record IkAimHarnessResult(
    FrameAnalysis Base,
    FrameAnalysis AimOnly,
    FrameAnalysis IkOnly,
    FrameAnalysis Combined,
    ulong BaseFingerprint,
    ulong AimOnlyFingerprint,
    ulong IkOnlyFingerprint,
    ulong CombinedFingerprint,
    float IkOnlyEndEffectorError,
    float CombinedEndEffectorError);

internal readonly record struct FrameAnalysis(
    int RenderedPixels,
    int FirstSectionPixels,
    int SecondSectionPixels,
    int MinimumX,
    int MinimumY,
    int MaximumX,
    int MaximumY,
    float CentroidX,
    float CentroidY);

internal readonly record struct SkeletonOverlayAnalysis(
    int ChangedPixels,
    int LinkPixels,
    int YAxisPixels);

internal readonly record struct CharacterAnimationFrame(
    float SampleTime,
    SkeletonGlobalPose GlobalPose,
    SkinningPalette Palette);

internal static partial class SdlGpuCharacterHarness
{
    private static readonly SDL_FColor ClearColor = new() { r = 0.035f, g = 0.045f, b = 0.070f, a = 1.0f };
    private const float DeterministicAnimationSampleTime = 0.5f;
    private const float DeterministicAnimationLaterSampleTime = 1.0f;
    private const float DeterministicIdleTime = 1.25f;
    private const float DeterministicWalkTime = 0.5f;
    private const float DeterministicActionBodyTime = 0.75f;
    private const float DeterministicLayerWalkTime = 0.75f;

    internal static CharacterHarnessResult Run(
        SkeletalCharacterAsset asset,
        AnimationClip animation,
        CharacterHarnessOptions options)
    {
        ArgumentNullException.ThrowIfNull(asset);
        ArgumentNullException.ThrowIfNull(animation);
        ArgumentNullException.ThrowIfNull(options);
        if (options.Width < 64 || options.Height < 64)
            throw new ArgumentOutOfRangeException(nameof(options), "The GPU harness target must be at least 64x64.");
        if (!ReferenceEquals(animation.Skeleton, asset.Mesh.Skin.Skeleton))
            throw new ArgumentException($"Animation '{animation.Name}' does not use the selected mesh skeleton.", nameof(animation));

        AnimationClip idleAnimation = SelectAnimation(asset, "Idle_Loop");
        AnimationClip walkAnimation = SelectAnimation(asset, "Walk_Loop");
        AnimationClip actionAnimation = SelectAnimation(asset, "Sword_Attack");
        SkeletonJointMask upperBodyMask = CreateUpperBodyMask(asset.Mesh.Skin.Skeleton);

        MeshBounds bounds = MeshBounds.Create(asset.Mesh.Vertices.Select(static vertex => vertex.Position).ToArray());
        SkeletonPose bindPose = asset.Mesh.Skin.Skeleton.CreateBindPose();
        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(bindPose);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(asset.Mesh.Skin, globalPose);
        BindPoseCamera camera = BindPoseCamera.Create(bounds, options.Width, options.Height);
        float skeletonAxisLength = bounds.Radius * 0.04f;
        SkeletonDebugGeometry skeletonDebug = SkeletonDebugGeometry.Create(globalPose, skeletonAxisLength);

        using var gpu = new CharacterGpuSession(asset.Mesh, skeletonDebug, options.Width, options.Height, options.Visible);

        gpu.UploadPalette(palette);
        byte[] bindPixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis bindAnalysis = Analyze(bindPixels, options.Width, options.Height, "bind-pose");

        float probeDistance = bounds.Radius * 0.08f;
        gpu.UploadPalette(CreateTranslatedPalette(palette, probeDistance));
        byte[] probePixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis probeAnalysis = Analyze(probePixels, options.Width, options.Height, "translated-palette");

        gpu.UploadPalette(palette);
        byte[] skeletonPixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: true);
        FrameAnalysis skeletonAnalysis = Analyze(skeletonPixels, options.Width, options.Height, "skeleton-debug");
        SkeletonOverlayAnalysis skeletonOverlay = AnalyzeSkeletonOverlay(bindPixels, skeletonPixels, options.Width, options.Height);

        CharacterAnimationFrame animationStart = CreateAnimationFrame(asset.Mesh.Skin, animation, 0.0f);
        gpu.UploadPalette(animationStart.Palette);
        byte[] animationStartPixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis animationStartAnalysis = Analyze(animationStartPixels, options.Width, options.Height, "animation-start");

        CharacterAnimationFrame animationSample = CreateAnimationFrame(asset.Mesh.Skin, animation, DeterministicAnimationSampleTime);
        gpu.UploadPalette(animationSample.Palette);
        byte[] animationSamplePixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis animationSampleAnalysis = Analyze(animationSamplePixels, options.Width, options.Height, "animation-sample");

        CharacterAnimationFrame animationLaterSample = CreateAnimationFrame(asset.Mesh.Skin, animation, DeterministicAnimationLaterSampleTime);
        gpu.UploadPalette(animationLaterSample.Palette);
        byte[] animationLaterSamplePixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis animationLaterSampleAnalysis = Analyze(
            animationLaterSamplePixels,
            options.Width,
            options.Height,
            "animation-later-sample");

        CharacterAnimationFrame animationLoopBoundary = CreateAnimationFrame(asset.Mesh.Skin, animation, animation.Duration);
        gpu.UploadPalette(animationLoopBoundary.Palette);
        byte[] animationLoopBoundaryPixels = gpu.RenderOffscreen(camera.ViewProjection, includeSkeleton: false);
        FrameAnalysis animationLoopBoundaryAnalysis = Analyze(animationLoopBoundaryPixels, options.Width, options.Height, "animation-loop-boundary");

        ulong bindFingerprint = Fingerprint(bindPixels);
        ulong probeFingerprint = Fingerprint(probePixels);
        ulong skeletonFingerprint = Fingerprint(skeletonPixels);
        ulong animationStartFingerprint = Fingerprint(animationStartPixels);
        ulong animationSampleFingerprint = Fingerprint(animationSamplePixels);
        ulong animationLaterSampleFingerprint = Fingerprint(animationLaterSamplePixels);
        ulong animationLoopBoundaryFingerprint = Fingerprint(animationLoopBoundaryPixels);
        Require(bindFingerprint != probeFingerprint, "Translated palette probe produced the bind-pose fingerprint.");
        Require(bindFingerprint != skeletonFingerprint, "Skeleton debug overlay produced the bind-pose fingerprint.");
        Require(
            animationStartFingerprint == animationLoopBoundaryFingerprint,
            $"Animation loop boundary {animationLoopBoundaryFingerprint:x16} did not reproduce start {animationStartFingerprint:x16}.");
        Require(animationSampleFingerprint != animationStartFingerprint, "Animation sample produced the loop-start fingerprint.");
        Require(animationSampleFingerprint != bindFingerprint, "Animation sample produced the bind-pose fingerprint.");
        Require(animationLaterSampleFingerprint != animationStartFingerprint, "Later animation sample produced the loop-start fingerprint.");
        Require(animationLaterSampleFingerprint != bindFingerprint, "Later animation sample produced the bind-pose fingerprint.");
        Require(animationLaterSampleFingerprint != animationSampleFingerprint, "Animation samples at 0.5 and 1.0 seconds produced the same fingerprint.");
        Require(
            MathF.Abs(probeAnalysis.CentroidX - bindAnalysis.CentroidX) >= options.Width * 0.025f,
            $"Translated palette probe shifted the rendered centroid by only {MathF.Abs(probeAnalysis.CentroidX - bindAnalysis.CentroidX):F2} pixels.");

        SkeletonPose idlePose = AnimationSampler.Sample(
            idleAnimation,
            DeterministicIdleTime,
            AnimationPlaybackMode.Loop);
        SkeletonPose walkPose = AnimationSampler.Sample(
            walkAnimation,
            DeterministicWalkTime,
            AnimationPlaybackMode.Loop);
        SkeletonPose locomotionMidpointPose = SkeletonPoseBlender.Blend(idlePose, walkPose, 0.5f);
        SkeletonPose actionEntryPose = SkeletonPoseBlender.Blend(
            walkPose,
            AnimationSampler.Sample(
                actionAnimation,
                CharacterPlaybackController.ActionBlendInDuration * 0.5f,
                AnimationPlaybackMode.Clamp),
            0.5f);
        SkeletonPose actionBodyPose = AnimationSampler.Sample(
            actionAnimation,
            DeterministicActionBodyTime,
            AnimationPlaybackMode.Clamp);
        float actionReturnTime = actionAnimation.Duration - CharacterPlaybackController.ActionBlendOutDuration * 0.5f;
        SkeletonPose actionReturnPose = SkeletonPoseBlender.Blend(
            AnimationSampler.Sample(actionAnimation, actionReturnTime, AnimationPlaybackMode.Clamp),
            AnimationSampler.Sample(
                walkAnimation,
                DeterministicAnimationLaterSampleTime,
                AnimationPlaybackMode.Loop),
            0.5f);

        byte[] locomotionIdlePixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            idlePose,
            DeterministicIdleTime,
            camera.ViewProjection,
            options,
            "blend-locomotion-idle",
            out FrameAnalysis locomotionIdleAnalysis);
        byte[] locomotionMidpointPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            locomotionMidpointPose,
            DeterministicWalkTime,
            camera.ViewProjection,
            options,
            "blend-locomotion-midpoint",
            out FrameAnalysis locomotionMidpointAnalysis);
        byte[] locomotionWalkPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            walkPose,
            DeterministicWalkTime,
            camera.ViewProjection,
            options,
            "blend-locomotion-walk",
            out FrameAnalysis locomotionWalkAnalysis);
        byte[] actionEntryPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            actionEntryPose,
            CharacterPlaybackController.ActionBlendInDuration * 0.5f,
            camera.ViewProjection,
            options,
            "blend-action-entry",
            out FrameAnalysis actionEntryAnalysis);
        byte[] actionBodyPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            actionBodyPose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "blend-action-body",
            out FrameAnalysis actionBodyAnalysis);
        byte[] actionReturnPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            actionReturnPose,
            actionReturnTime,
            camera.ViewProjection,
            options,
            "blend-action-return",
            out FrameAnalysis actionReturnAnalysis);

        var blendResult = new BlendHarnessResult(
            locomotionIdleAnalysis,
            locomotionMidpointAnalysis,
            locomotionWalkAnalysis,
            actionEntryAnalysis,
            actionBodyAnalysis,
            actionReturnAnalysis,
            Fingerprint(locomotionIdlePixels),
            Fingerprint(locomotionMidpointPixels),
            Fingerprint(locomotionWalkPixels),
            Fingerprint(actionEntryPixels),
            Fingerprint(actionBodyPixels),
            Fingerprint(actionReturnPixels));
        Require(
            blendResult.LocomotionMidpointFingerprint != blendResult.LocomotionIdleFingerprint &&
            blendResult.LocomotionMidpointFingerprint != blendResult.LocomotionWalkFingerprint,
            "Locomotion blend midpoint reproduced an endpoint fingerprint.");
        Require(
            blendResult.ActionEntryFingerprint != blendResult.LocomotionWalkFingerprint,
            "Action entry blend reproduced the locomotion fingerprint.");
        Require(
            blendResult.ActionBodyFingerprint != blendResult.ActionEntryFingerprint &&
            blendResult.ActionBodyFingerprint != blendResult.ActionReturnFingerprint,
            "Action blend frames did not produce distinct fingerprints.");

        SkeletonPose layerWalkBasePose = AnimationSampler.Sample(
            walkAnimation,
            DeterministicLayerWalkTime,
            AnimationPlaybackMode.Loop);
        SkeletonPose layerFullBodyActionPose = AnimationSampler.Sample(
            actionAnimation,
            DeterministicActionBodyTime,
            AnimationPlaybackMode.Clamp);
        SkeletonPose layerUpperBodyActionPose = SkeletonPoseLayerer.Apply(
            layerWalkBasePose,
            layerFullBodyActionPose,
            upperBodyMask,
            1.0f);
        float layeredEntryActionTime = CharacterPlaybackController.ActionBlendInDuration * 0.5f;
        SkeletonPose layerEntryAdvancingBase = AnimationSampler.Sample(
            walkAnimation,
            DeterministicWalkTime + layeredEntryActionTime,
            AnimationPlaybackMode.Loop);
        SkeletonPose layerEntryDisplayedSource = SkeletonPoseLayerer.Apply(
            layerEntryAdvancingBase,
            AnimationSampler.Sample(
                walkAnimation,
                DeterministicWalkTime,
                AnimationPlaybackMode.Loop),
            upperBodyMask,
            1.0f);
        SkeletonPose layerEntryPose = SkeletonPoseLayerer.Apply(
            layerEntryDisplayedSource,
            AnimationSampler.Sample(
                actionAnimation,
                layeredEntryActionTime,
                AnimationPlaybackMode.Clamp),
            upperBodyMask,
            0.5f);
        SkeletonPose layerWalkAdvancedPose = AnimationSampler.Sample(
            walkAnimation,
            DeterministicAnimationLaterSampleTime,
            AnimationPlaybackMode.Loop);
        SkeletonPose layerReturnPose = SkeletonPoseLayerer.Apply(
            layerWalkAdvancedPose,
            AnimationSampler.Sample(actionAnimation, actionReturnTime, AnimationPlaybackMode.Clamp),
            upperBodyMask,
            0.5f);

        ValidateLayeredPose(layerWalkBasePose, layerFullBodyActionPose, layerUpperBodyActionPose, upperBodyMask);

        byte[] layerWalkBasePixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerWalkBasePose,
            DeterministicLayerWalkTime,
            camera.ViewProjection,
            options,
            "layer-walk-base",
            out FrameAnalysis layerWalkBaseAnalysis);
        byte[] layerFullBodyActionPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerFullBodyActionPose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "layer-full-action",
            out FrameAnalysis layerFullBodyActionAnalysis);
        byte[] layerUpperBodyActionPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerUpperBodyActionPose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "layer-upper-action",
            out FrameAnalysis layerUpperBodyActionAnalysis);
        byte[] layerActionEntryPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerEntryPose,
            layeredEntryActionTime,
            camera.ViewProjection,
            options,
            "layer-action-entry",
            out FrameAnalysis layerActionEntryAnalysis);
        byte[] layerActionReturnPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerReturnPose,
            actionReturnTime,
            camera.ViewProjection,
            options,
            "layer-action-return",
            out FrameAnalysis layerActionReturnAnalysis);
        byte[] layerWalkAdvancedPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            layerWalkAdvancedPose,
            DeterministicAnimationLaterSampleTime,
            camera.ViewProjection,
            options,
            "layer-walk-advanced",
            out FrameAnalysis layerWalkAdvancedAnalysis);

        var layeredResult = new LayeredHarnessResult(
            layerWalkBaseAnalysis,
            layerFullBodyActionAnalysis,
            layerUpperBodyActionAnalysis,
            layerActionEntryAnalysis,
            layerActionReturnAnalysis,
            layerWalkAdvancedAnalysis,
            Fingerprint(layerWalkBasePixels),
            Fingerprint(layerFullBodyActionPixels),
            Fingerprint(layerUpperBodyActionPixels),
            Fingerprint(layerActionEntryPixels),
            Fingerprint(layerActionReturnPixels),
            Fingerprint(layerWalkAdvancedPixels),
            FindJointIndex(asset.Mesh.Skin.Skeleton, "spine_01"),
            upperBodyMask.IncludedJointCount);
        Require(
            layeredResult.UpperBodyActionFingerprint != layeredResult.WalkBaseFingerprint &&
            layeredResult.UpperBodyActionFingerprint != layeredResult.FullBodyActionFingerprint,
            "Upper-body layered action reproduced one of its source fingerprints.");
        Require(
            layeredResult.ActionEntryFingerprint != layeredResult.UpperBodyActionFingerprint &&
            layeredResult.ActionReturnFingerprint != layeredResult.UpperBodyActionFingerprint,
            "Layered transition frames reproduced the action-body fingerprint.");
        Require(
            layeredResult.WalkBaseFingerprint != layeredResult.WalkAdvancedFingerprint,
            "Layered evidence walk timestamps produced the same fingerprint.");

        var ikAimPresentation = new SelectedIkAimPresentation(asset.Mesh.Skin.Skeleton);
        SelectedIkAimPose aimOnlyPose = ikAimPresentation.Apply(
            actionBodyPose,
            applyAim: true,
            applyIk: false);
        SelectedIkAimPose ikOnlyPose = ikAimPresentation.Apply(
            actionBodyPose,
            applyAim: false,
            applyIk: true);
        SelectedIkAimPose combinedPose = ikAimPresentation.Apply(
            actionBodyPose,
            applyAim: true,
            applyIk: true);
        byte[] aimOnlyPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            aimOnlyPose.Pose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "ik-aim-aim-only",
            out FrameAnalysis aimOnlyAnalysis);
        byte[] ikOnlyPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            ikOnlyPose.Pose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "ik-aim-ik-only",
            out FrameAnalysis ikOnlyAnalysis);
        byte[] combinedPixels = RenderPose(
            gpu,
            asset.Mesh.Skin,
            combinedPose.Pose,
            DeterministicActionBodyTime,
            camera.ViewProjection,
            options,
            "ik-aim-combined",
            out FrameAnalysis combinedAnalysis);
        var ikAimResult = new IkAimHarnessResult(
            actionBodyAnalysis,
            aimOnlyAnalysis,
            ikOnlyAnalysis,
            combinedAnalysis,
            Fingerprint(actionBodyPixels),
            Fingerprint(aimOnlyPixels),
            Fingerprint(ikOnlyPixels),
            Fingerprint(combinedPixels),
            ikOnlyPose.EndEffectorError,
            combinedPose.EndEffectorError);
        Require(
            ikAimResult.IkOnlyEndEffectorError <= 0.002f &&
            ikAimResult.CombinedEndEffectorError <= 0.002f,
            $"Selected off-hand IK error exceeded tolerance: " +
            $"{ikAimResult.IkOnlyEndEffectorError:F6}/{ikAimResult.CombinedEndEffectorError:F6} metres.");
        Require(
            ikAimResult.AimOnlyFingerprint != ikAimResult.BaseFingerprint &&
            ikAimResult.IkOnlyFingerprint != ikAimResult.BaseFingerprint &&
            ikAimResult.CombinedFingerprint != ikAimResult.BaseFingerprint,
            "An IK/Aim proof frame reproduced the base fingerprint.");
        Require(
            ikAimResult.AimOnlyFingerprint != ikAimResult.IkOnlyFingerprint &&
            ikAimResult.AimOnlyFingerprint != ikAimResult.CombinedFingerprint &&
            ikAimResult.IkOnlyFingerprint != ikAimResult.CombinedFingerprint,
            "IK/Aim proof modes did not produce distinct fingerprints.");

        if (!string.IsNullOrWhiteSpace(options.CapturePath))
            WritePpm(options.CapturePath, options.Width, options.Height, bindPixels);
        if (!string.IsNullOrWhiteSpace(options.SkeletonCapturePath))
            WritePpm(options.SkeletonCapturePath, options.Width, options.Height, skeletonPixels);
        if (!string.IsNullOrWhiteSpace(options.AnimationCapturePath))
            WritePpm(options.AnimationCapturePath, options.Width, options.Height, animationSamplePixels);
        if (!string.IsNullOrWhiteSpace(options.CaptureSuiteDirectory))
        {
            WriteCaptureSuite(
                options.CaptureSuiteDirectory,
                options.Width,
                options.Height,
                bindPixels,
                animationStartPixels,
                animationSamplePixels,
                animationLaterSamplePixels,
                animationLoopBoundaryPixels);
        }
        if (!string.IsNullOrWhiteSpace(options.BlendCaptureSuiteDirectory))
        {
            WriteBlendCaptureSuite(
                options.BlendCaptureSuiteDirectory,
                options.Width,
                options.Height,
                locomotionIdlePixels,
                locomotionMidpointPixels,
                locomotionWalkPixels,
                actionEntryPixels,
                actionBodyPixels,
                actionReturnPixels);
        }
        if (!string.IsNullOrWhiteSpace(options.LayeredCaptureSuiteDirectory))
        {
            WriteLayeredCaptureSuite(
                options.LayeredCaptureSuiteDirectory,
                options.Width,
                options.Height,
                layerWalkBasePixels,
                layerFullBodyActionPixels,
                layerUpperBodyActionPixels,
                layerActionEntryPixels,
                layerActionReturnPixels,
                layerWalkAdvancedPixels);
        }
        if (!string.IsNullOrWhiteSpace(options.IkAimCaptureSuiteDirectory))
        {
            WriteIkAimCaptureSuite(
                options.IkAimCaptureSuiteDirectory,
                options.Width,
                options.Height,
                actionBodyPixels,
                aimOnlyPixels,
                ikOnlyPixels,
                combinedPixels);
        }

        Console.WriteLine(
            $"GPU_HARNESS_PASS bind-pose shader={gpu.ShaderFormat} pixels={bindAnalysis.RenderedPixels} " +
            $"sections={bindAnalysis.FirstSectionPixels}/{bindAnalysis.SecondSectionPixels} " +
            $"bounds={bindAnalysis.MinimumX},{bindAnalysis.MinimumY}-{bindAnalysis.MaximumX},{bindAnalysis.MaximumY} " +
            $"fingerprint={bindFingerprint:x16}");
        Console.WriteLine(
            $"GPU_HARNESS_PASS palette-probe shift={probeAnalysis.CentroidX - bindAnalysis.CentroidX:F2} " +
            $"fingerprint={probeFingerprint:x16}");
        Console.WriteLine(
            $"GPU_HARNESS_PASS skeleton-debug lines={skeletonDebug.LineCount} changed={skeletonOverlay.ChangedPixels} " +
            $"links={skeletonOverlay.LinkPixels} y-axes={skeletonOverlay.YAxisPixels} " +
            $"fingerprint={skeletonFingerprint:x16}");
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"GPU_HARNESS_PASS animation clip={animation.Name} sample={DeterministicAnimationSampleTime:F3} " +
            $"duration={animation.Duration:F6} start={animationStartFingerprint:x16} " +
            $"sample-fingerprint={animationSampleFingerprint:x16} " +
            $"later-sample={DeterministicAnimationLaterSampleTime:F3} " +
            $"later-fingerprint={animationLaterSampleFingerprint:x16} loop={animationLoopBoundaryFingerprint:x16}"));
        Console.WriteLine(
            $"GPU_HARNESS_PASS blending locomotion={blendResult.LocomotionIdleFingerprint:x16}/" +
            $"{blendResult.LocomotionMidpointFingerprint:x16}/{blendResult.LocomotionWalkFingerprint:x16} " +
            $"action={blendResult.ActionEntryFingerprint:x16}/{blendResult.ActionBodyFingerprint:x16}/" +
            $"{blendResult.ActionReturnFingerprint:x16}");
        Console.WriteLine(
            $"GPU_HARNESS_PASS layering mask=spine_01:{layeredResult.MaskedJointCount}/{asset.Mesh.Skin.Skeleton.JointCount} " +
            $"comparison={layeredResult.WalkBaseFingerprint:x16}/{layeredResult.FullBodyActionFingerprint:x16}/" +
            $"{layeredResult.UpperBodyActionFingerprint:x16} transition={layeredResult.ActionEntryFingerprint:x16}/" +
            $"{layeredResult.ActionReturnFingerprint:x16}/{layeredResult.WalkAdvancedFingerprint:x16}");
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"GPU_HARNESS_PASS ik-aim fingerprints={ikAimResult.BaseFingerprint:x16}/" +
            $"{ikAimResult.AimOnlyFingerprint:x16}/{ikAimResult.IkOnlyFingerprint:x16}/" +
            $"{ikAimResult.CombinedFingerprint:x16} error=" +
            $"{ikAimResult.IkOnlyEndEffectorError:F6}/{ikAimResult.CombinedEndEffectorError:F6}"));

        if (options.Visible)
        {
            Console.WriteLine($"GPU_HARNESS_VISIBLE Playing {animation.Name} at normal speed with root motion disabled. Close the window or press Escape after inspection.");
            gpu.RunVisible(
                camera.ViewProjection,
                asset.Animations,
                animation,
                asset.Mesh.Skin,
                upperBodyMask,
                ikAimPresentation,
                skeletonAxisLength);
        }

        return new CharacterHarnessResult(
            gpu.ShaderFormat,
            bindAnalysis,
            probeAnalysis,
            skeletonAnalysis,
            animationStartAnalysis,
            animationSampleAnalysis,
            animationLaterSampleAnalysis,
            animationLoopBoundaryAnalysis,
            skeletonOverlay,
            bindFingerprint,
            probeFingerprint,
            skeletonFingerprint,
            animationStartFingerprint,
            animationSampleFingerprint,
            animationLaterSampleFingerprint,
            animationLoopBoundaryFingerprint,
            blendResult,
            layeredResult,
            ikAimResult,
            skeletonDebug.LineCount);
    }

    private static SkeletonJointMask CreateUpperBodyMask(SkeletonDefinition skeleton)
    {
        int rootIndex = FindJointIndex(skeleton, "spine_01");
        SkeletonJointMask mask = SkeletonJointMask.CreateSubtree(skeleton, rootIndex);
        Require(
            mask.IncludedJointCount == 53,
            $"Expected the selected spine_01 subtree to contain 53 joints, but found {mask.IncludedJointCount}.");
        return mask;
    }

    private static int FindJointIndex(SkeletonDefinition skeleton, string name)
    {
        if (skeleton.TryGetJointIndex(name, out int index))
            return index;
        throw new InvalidOperationException($"Required selected-skeleton joint '{name}' was not found.");
    }

    private static void ValidateLayeredPose(
        SkeletonPose basePose,
        SkeletonPose layerPose,
        SkeletonPose result,
        SkeletonJointMask mask)
    {
        for (int index = 0; index < mask.Skeleton.JointCount; index++)
        {
            JointTransform expected = mask[index]
                ? layerPose.LocalTransforms[index]
                : basePose.LocalTransforms[index];
            Require(
                result.LocalTransforms[index] == expected,
                $"Layered pose joint {index} did not preserve the selected endpoint transform.");
        }
    }

    private static AnimationClip SelectAnimation(SkeletalCharacterAsset asset, string name)
    {
        AnimationClip? selected = asset.Animations.SingleOrDefault(
            candidate => string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if (selected is not null)
            return selected;

        string available = string.Join(", ", asset.Animations.Select(static candidate => candidate.Name));
        throw new InvalidOperationException(
            $"Required blend animation '{name}' was not found by ordinal name. Available clips: {available}");
    }

    private static byte[] RenderPose(
        CharacterGpuSession gpu,
        SkinDefinition skin,
        SkeletonPose pose,
        float sampleTime,
        Matrix4x4 viewProjection,
        CharacterHarnessOptions options,
        string label,
        out FrameAnalysis analysis)
    {
        CharacterAnimationFrame frame = CreateAnimationFrame(skin, pose, sampleTime);
        gpu.UploadPalette(frame.Palette);
        byte[] pixels = gpu.RenderOffscreen(viewProjection, includeSkeleton: false);
        analysis = Analyze(pixels, options.Width, options.Height, label);
        return pixels;
    }

    internal static CharacterAnimationFrame CreateAnimationFrame(
        SkinDefinition skin,
        AnimationClip animation,
        float time)
    {
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(animation);
        if (!ReferenceEquals(skin.Skeleton, animation.Skeleton))
            throw new ArgumentException($"Animation '{animation.Name}' does not use the skin skeleton.", nameof(animation));

        float sampleTime = AnimationSampler.ResolveTime(animation, time, AnimationPlaybackMode.Loop);
        SkeletonPose pose = AnimationSampler.Sample(animation, sampleTime, AnimationPlaybackMode.Clamp);
        return CreateAnimationFrame(skin, pose, sampleTime);
    }

    internal static CharacterAnimationFrame CreateAnimationFrame(
        SkinDefinition skin,
        SkeletonPose pose,
        float sampleTime)
    {
        ArgumentNullException.ThrowIfNull(skin);
        ArgumentNullException.ThrowIfNull(pose);
        if (!float.IsFinite(sampleTime) || sampleTime < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(sampleTime));
        if (!ReferenceEquals(skin.Skeleton, pose.Skeleton))
            throw new ArgumentException("Pose does not use the selected skin skeleton.", nameof(pose));

        SkeletonGlobalPose globalPose = SkeletonPoseEvaluator.EvaluateGlobal(pose);
        SkinningPalette palette = SkeletonPoseEvaluator.CreateSkinningPalette(skin, globalPose);
        return new CharacterAnimationFrame(sampleTime, globalPose, palette);
    }

    private static SkinningPalette CreateTranslatedPalette(SkinningPalette palette, float distance)
    {
        Matrix4x4 translation = Matrix4x4.CreateTranslation(distance, 0.0f, 0.0f);
        return new SkinningPalette(
            palette.Skin,
            palette.JointMatrices.Select(matrix => matrix * translation));
    }

    private static SkeletonOverlayAnalysis AnalyzeSkeletonOverlay(
        byte[] baseline,
        byte[] overlay,
        int width,
        int height)
    {
        int expectedLength = checked(width * height * 4);
        Require(baseline.Length == expectedLength, $"skeleton-debug: unexpected baseline byte count {baseline.Length}.");
        Require(overlay.Length == expectedLength, $"skeleton-debug: unexpected overlay byte count {overlay.Length}.");

        int changedPixels = 0;
        int linkPixels = 0;
        int yAxisPixels = 0;
        for (int offset = 0; offset < overlay.Length; offset += 4)
        {
            byte red = overlay[offset];
            byte green = overlay[offset + 1];
            byte blue = overlay[offset + 2];
            if (Math.Abs(red - baseline[offset]) > 3 ||
                Math.Abs(green - baseline[offset + 1]) > 3 ||
                Math.Abs(blue - baseline[offset + 2]) > 3)
            {
                changedPixels++;
            }

            if (red > 180 && green > 150 && blue < 100)
                linkPixels++;
            if (green > 180 && red < 100 && blue < 100)
                yAxisPixels++;
        }

        Require(changedPixels > 100, $"skeleton-debug: only {changedPixels} pixels changed from the bind-pose frame.");
        Require(linkPixels > 25, $"skeleton-debug: only {linkPixels} hierarchy-link pixels were classified.");
        Require(yAxisPixels > 25, $"skeleton-debug: only {yAxisPixels} Y-axis pixels were classified.");
        return new SkeletonOverlayAnalysis(changedPixels, linkPixels, yAxisPixels);
    }

    private static FrameAnalysis Analyze(byte[] rgba, int width, int height, string passName)
    {
        Require(rgba.Length == checked(width * height * 4), $"{passName}: unexpected RGBA byte count {rgba.Length}.");
        byte clearRed = ToByte(ClearColor.r);
        byte clearGreen = ToByte(ClearColor.g);
        byte clearBlue = ToByte(ClearColor.b);
        int rendered = 0;
        int firstSection = 0;
        int secondSection = 0;
        int minimumX = width;
        int minimumY = height;
        int maximumX = -1;
        int maximumY = -1;
        long xTotal = 0;
        long yTotal = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int offset = (y * width + x) * 4;
                byte red = rgba[offset];
                byte green = rgba[offset + 1];
                byte blue = rgba[offset + 2];
                Require(rgba[offset + 3] == byte.MaxValue, $"{passName}: pixel {x},{y} was not opaque.");
                bool differs = Math.Abs(red - clearRed) > 3 || Math.Abs(green - clearGreen) > 3 || Math.Abs(blue - clearBlue) > 3;
                if (!differs)
                    continue;

                rendered++;
                xTotal += x;
                yTotal += y;
                minimumX = Math.Min(minimumX, x);
                minimumY = Math.Min(minimumY, y);
                maximumX = Math.Max(maximumX, x);
                maximumY = Math.Max(maximumY, y);
                if (red > blue + 18 && red > green)
                    firstSection++;
                if (blue > red + 18 && blue > green)
                    secondSection++;
            }
        }

        Require(rendered > width * height / 80, $"{passName}: only {rendered} pixels differed from the clear color.");
        Require(firstSection > 100, $"{passName}: first diagnostic section produced only {firstSection} classified pixels.");
        Require(secondSection > 100, $"{passName}: second diagnostic section produced only {secondSection} classified pixels.");
        Require(minimumX > 4 && minimumY > 4 && maximumX < width - 5 && maximumY < height - 5,
            $"{passName}: rendered bounds {minimumX},{minimumY}-{maximumX},{maximumY} touch the target edge.");
        float centroidX = xTotal / (float)rendered;
        float centroidY = yTotal / (float)rendered;
        Require(MathF.Abs(centroidX - (width - 1) * 0.5f) < width * 0.16f,
            $"{passName}: horizontal centroid {centroidX:F2} is not centered.");
        Require(MathF.Abs(centroidY - (height - 1) * 0.5f) < height * 0.16f,
            $"{passName}: vertical centroid {centroidY:F2} is not centered.");

        return new FrameAnalysis(
            rendered,
            firstSection,
            secondSection,
            minimumX,
            minimumY,
            maximumX,
            maximumY,
            centroidX,
            centroidY);
    }

    private static ulong Fingerprint(ReadOnlySpan<byte> bytes)
    {
        const ulong offsetBasis = 14695981039346656037;
        const ulong prime = 1099511628211;
        ulong hash = offsetBasis;
        foreach (byte value in bytes)
        {
            hash ^= value;
            hash *= prime;
        }
        return hash;
    }

    private static void WritePpm(string path, int width, int height, byte[] rgba)
    {
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        byte[] header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        byte[] output = new byte[checked(header.Length + width * height * 3)];
        header.CopyTo(output, 0);
        int destination = header.Length;
        for (int source = 0; source < rgba.Length; source += 4)
        {
            output[destination++] = rgba[source];
            output[destination++] = rgba[source + 1];
            output[destination++] = rgba[source + 2];
        }
        File.WriteAllBytes(fullPath, output);
        Console.WriteLine($"GPU_HARNESS_CAPTURE {fullPath}");
    }

    private static void WriteCaptureSuite(
        string directory,
        int width,
        int height,
        byte[] bindPose,
        byte[] animationStart,
        byte[] animationSample,
        byte[] animationLaterSample,
        byte[] animationLoopBoundary)
    {
        string fullDirectory = Path.GetFullPath(directory);
        WritePpm(Path.Combine(fullDirectory, "bind-pose.ppm"), width, height, bindPose);
        WritePpm(Path.Combine(fullDirectory, "animation-0000ms.ppm"), width, height, animationStart);
        WritePpm(Path.Combine(fullDirectory, "animation-0500ms.ppm"), width, height, animationSample);
        WritePpm(Path.Combine(fullDirectory, "animation-1000ms.ppm"), width, height, animationLaterSample);
        WritePpm(Path.Combine(fullDirectory, "animation-loop-boundary.ppm"), width, height, animationLoopBoundary);
        Console.WriteLine($"GPU_HARNESS_CAPTURE_SUITE {fullDirectory}");
    }

    private static void WriteBlendCaptureSuite(
        string directory,
        int width,
        int height,
        byte[] locomotionIdle,
        byte[] locomotionMidpoint,
        byte[] locomotionWalk,
        byte[] actionEntry,
        byte[] actionBody,
        byte[] actionReturn)
    {
        string fullDirectory = Path.GetFullPath(directory);
        WritePpm(Path.Combine(fullDirectory, "blend-locomotion-idle.ppm"), width, height, locomotionIdle);
        WritePpm(Path.Combine(fullDirectory, "blend-locomotion-midpoint.ppm"), width, height, locomotionMidpoint);
        WritePpm(Path.Combine(fullDirectory, "blend-locomotion-walk.ppm"), width, height, locomotionWalk);
        WritePpm(Path.Combine(fullDirectory, "blend-action-entry.ppm"), width, height, actionEntry);
        WritePpm(Path.Combine(fullDirectory, "blend-action-body.ppm"), width, height, actionBody);
        WritePpm(Path.Combine(fullDirectory, "blend-action-return.ppm"), width, height, actionReturn);
        Console.WriteLine($"GPU_HARNESS_BLEND_CAPTURE_SUITE {fullDirectory}");
    }

    private static void WriteLayeredCaptureSuite(
        string directory,
        int width,
        int height,
        byte[] walkBase,
        byte[] fullBodyAction,
        byte[] upperBodyAction,
        byte[] actionEntry,
        byte[] actionReturn,
        byte[] walkAdvanced)
    {
        string fullDirectory = Path.GetFullPath(directory);
        WritePpm(Path.Combine(fullDirectory, "layer-walk-base.ppm"), width, height, walkBase);
        WritePpm(Path.Combine(fullDirectory, "layer-full-action.ppm"), width, height, fullBodyAction);
        WritePpm(Path.Combine(fullDirectory, "layer-upper-action.ppm"), width, height, upperBodyAction);
        WritePpm(Path.Combine(fullDirectory, "layer-action-entry.ppm"), width, height, actionEntry);
        WritePpm(Path.Combine(fullDirectory, "layer-action-return.ppm"), width, height, actionReturn);
        WritePpm(Path.Combine(fullDirectory, "layer-walk-advanced.ppm"), width, height, walkAdvanced);
        Console.WriteLine($"GPU_HARNESS_LAYERED_CAPTURE_SUITE {fullDirectory}");
    }

    private static void WriteIkAimCaptureSuite(
        string directory,
        int width,
        int height,
        byte[] basePose,
        byte[] aimOnly,
        byte[] ikOnly,
        byte[] combined)
    {
        string fullDirectory = Path.GetFullPath(directory);
        WritePpm(Path.Combine(fullDirectory, "ik-aim-base.ppm"), width, height, basePose);
        WritePpm(Path.Combine(fullDirectory, "ik-aim-aim-only.ppm"), width, height, aimOnly);
        WritePpm(Path.Combine(fullDirectory, "ik-aim-ik-only.ppm"), width, height, ikOnly);
        WritePpm(Path.Combine(fullDirectory, "ik-aim-combined.ppm"), width, height, combined);
        Console.WriteLine($"GPU_HARNESS_IK_AIM_CAPTURE_SUITE {fullDirectory}");
    }

    private static byte ToByte(float value) => (byte)MathF.Round(Math.Clamp(value, 0.0f, 1.0f) * byte.MaxValue);

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    internal static SDL_WindowFlags SelectWindowFlags(bool visible) =>
        visible ? (SDL_WindowFlags)0 : SDL_WindowFlags.SDL_WINDOW_HIDDEN;

    internal static void ExecuteInteractiveFrame(
        AnimationClip clip,
        float sampleTime,
        int jointCount,
        Action operation)
    {
        ArgumentNullException.ThrowIfNull(clip);
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            operation();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"Interactive animation validation failed for clip '{clip.Name}' " +
                    $"at sample {sampleTime:F3} seconds (joints={jointCount})."),
                exception);
        }
    }

    private sealed unsafe partial class CharacterGpuSession : IDisposable
    {
        private const SDL_GPUTextureFormat DepthFormat = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D32_FLOAT;
        private readonly int width;
        private readonly int height;
        private SDL_Window* window;
        private SDL_GPUDevice* device;
        private SdlGpuSkinnedCharacterRenderer? characterRenderer;
        private SdlGpuSkinnedMesh? characterMesh;
        private SdlGpuSkinningPalette? characterPalette;
        private SdlGpuStaticMeshRenderer? attachmentRenderer;
        private SdlGpuStaticMesh? attachmentMesh;
        private SDL_GPUShader* skeletonVertexShader;
        private SDL_GPUShader* skeletonFragmentShader;
        private SDL_GPUGraphicsPipeline* skeletonPipeline;
        private SDL_GPUBuffer* skeletonVertexBuffer;
        private SDL_GPUTransferBuffer* skeletonTransferBuffer;
        private SDL_GPUTexture* offscreenColor;
        private SDL_GPUTexture* offscreenDepth;
        private SDL_GPUTexture* visibleDepth;
        private uint visibleDepthWidth;
        private uint visibleDepthHeight;
        private bool windowClaimed;
        private readonly int jointCount;
        private readonly uint skeletonVertexCount;

        internal CharacterGpuSession(
            SkinnedMeshDefinition mesh,
            SkeletonDebugGeometry skeletonDebug,
            int width,
            int height,
            bool visible,
            StaticMeshDefinition? attachmentSource = null)
        {
            ArgumentNullException.ThrowIfNull(mesh);
            ArgumentNullException.ThrowIfNull(skeletonDebug);
            this.width = width;
            this.height = height;
            jointCount = mesh.Skin.Skeleton.JointCount;
            skeletonVertexCount = checked((uint)skeletonDebug.Vertices.Length);

            if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
                throw new InvalidOperationException($"SDL video initialization failed: {SDL_GetError()}");

            try
            {
                SDL_WindowFlags flags = SelectWindowFlags(visible);
                window = SDL_CreateWindow("ChronoFall character animation experiment", width, height, flags);
                if (window is null)
                    throw new InvalidOperationException($"SDL window creation failed: {SDL_GetError()}");

                const SDL_GPUShaderFormat requested =
                    SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL |
                    SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;
                device = SDL_CreateGPUDevice(requested, debug_mode: true, name: (byte*)null);
                if (device is null)
                    throw new InvalidOperationException($"SDL GPU device creation failed: {SDL_GetError()}");
                if (!SDL_ClaimWindowForGPUDevice(device, window))
                    throw new InvalidOperationException($"SDL GPU window claim failed: {SDL_GetError()}");
                windowClaimed = true;

                ShaderFormat = ShaderAssetSelector.SelectPreferred(SDL_GetGPUShaderFormats(device));
                SDL_GPUTextureFormat colorFormat = SDL_GetGPUSwapchainTextureFormat(device, window);
                characterRenderer = new SdlGpuSkinnedCharacterRenderer(
                    device,
                    colorFormat,
                    DepthFormat,
                    LoadCharacterShaders());
                if (attachmentSource is not null)
                {
                    attachmentRenderer = new SdlGpuStaticMeshRenderer(
                        device,
                        colorFormat,
                        DepthFormat,
                        LoadStaticShaders());
                }
                SDL_GPUCommandBuffer* geometryCommand = AcquireCommand();
                characterMesh = characterRenderer.UploadMesh(geometryCommand, mesh);
                characterPalette = characterRenderer.CreatePalette(jointCount);
                if (attachmentSource is not null)
                    attachmentMesh = attachmentRenderer!.UploadMesh(geometryCommand, attachmentSource);
                if (!SDL_SubmitGPUCommandBuffer(geometryCommand))
                    throw new InvalidOperationException($"SDL GPU character geometry submission failed: {SDL_GetError()}");
                skeletonVertexShader = LoadShader("skeleton-debug.vert", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX, storageBuffers: 0, uniformBuffers: 1);
                skeletonFragmentShader = LoadShader("skeleton-debug.frag", SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT, storageBuffers: 0, uniformBuffers: 0);
                skeletonPipeline = CreateSkeletonPipeline(colorFormat);

                skeletonVertexBuffer = CreateBuffer(
                    SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX,
                    checked(skeletonVertexCount * GpuDebugLineVertex.Stride));
                skeletonTransferBuffer = CreateTransfer(
                    SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
                    checked(skeletonVertexCount * GpuDebugLineVertex.Stride));
                UploadBuffer(skeletonVertexBuffer, skeletonDebug.Vertices);

                offscreenColor = CreateTexture(colorFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_COLOR_TARGET, (uint)width, (uint)height);
                offscreenDepth = CreateTexture(DepthFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET, (uint)width, (uint)height);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        internal SDL_GPUShaderFormat ShaderFormat { get; }

        internal void UploadPalette(SkinningPalette palette)
        {
            ArgumentNullException.ThrowIfNull(palette);
            SDL_GPUCommandBuffer* command = AcquireCommand();
            characterRenderer!.UploadPalette(command, characterPalette!, palette);
            if (!SDL_SubmitGPUCommandBuffer(command))
                throw new InvalidOperationException($"SDL GPU palette upload submission failed: {SDL_GetError()}");
        }

        internal void UploadSkeleton(GpuDebugLineVertex[] vertices)
        {
            ArgumentNullException.ThrowIfNull(vertices);
            if (vertices.Length != skeletonVertexCount)
            {
                throw new ArgumentException(
                    $"Expected {skeletonVertexCount} skeleton vertices, received {vertices.Length}.",
                    nameof(vertices));
            }
            UploadCycled(skeletonTransferBuffer, skeletonVertexBuffer, vertices, "skeleton");
        }

        internal byte[] RenderOffscreen(
            Matrix4x4 viewProjection,
            bool includeSkeleton,
            Matrix4x4? attachmentWorld = null)
        {
            SDL_GPUCommandBuffer* command = AcquireCommand();
            try
            {
                Render(
                    command,
                    offscreenColor,
                    offscreenDepth,
                    (uint)width,
                    (uint)height,
                    viewProjection,
                    includeSkeleton,
                    attachmentWorld);
            }
            catch
            {
                _ = SDL_CancelGPUCommandBuffer(command);
                throw;
            }

            using SdlGpuReadbackRequest request = SdlGpuTextureReadback.Submit(
                device,
                command,
                offscreenColor,
                width,
                height,
                SDL_GetGPUSwapchainTextureFormat(device, window));
            return request.Wait().Pixels.ToArray();
        }

        internal void RunVisible(
            Matrix4x4 viewProjection,
            IReadOnlyList<AnimationClip> animations,
            AnimationClip initialAnimation,
            SkinDefinition skin,
            SkeletonJointMask upperBodyMask,
            SelectedIkAimPresentation ikAimPresentation,
            float skeletonAxisLength)
        {
            ArgumentNullException.ThrowIfNull(animations);
            ArgumentNullException.ThrowIfNull(initialAnimation);
            ArgumentNullException.ThrowIfNull(skin);
            ArgumentNullException.ThrowIfNull(upperBodyMask);
            ArgumentNullException.ThrowIfNull(ikAimPresentation);
            if (!ReferenceEquals(skin.Skeleton, upperBodyMask.Skeleton))
                throw new ArgumentException("The visible action mask must use the skin skeleton.", nameof(upperBodyMask));
            if (!SDL_ShowWindow(window))
                throw new InvalidOperationException($"SDL could not show the validation window: {SDL_GetError()}");

            var playback = new CharacterPlaybackController(animations, initialAnimation.Name);
            ulong frequency = SDL_GetPerformanceFrequency();
            if (frequency == 0)
                throw new InvalidOperationException("SDL returned a zero performance-counter frequency.");
            ulong previousCounter = SDL_GetPerformanceCounter();
            ulong lastTitleCounter = 0;
            bool titleDirty = true;
            bool aimEnabled = false;
            bool ikEnabled = false;
            Console.WriteLine(
                "GPU_HARNESS_CONTROLS Left/Right=direct-clip 1=blend-Idle_Loop 2=blend-Walk_Loop " +
                "3=signal-full-Sword_Attack 4=signal-layered-Sword_Attack " +
                "5=toggle-aim 6=toggle-off-hand-IK " +
                "Space=pause/resume R=restart D=skeleton Escape=close");
            Console.WriteLine(playback.CreateConsoleDiagnostic(jointCount, jointCount));
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
                        ApplyControl(
                            playback,
                            upperBodyMask,
                            ref aimEnabled,
                            ref ikEnabled,
                            sdlEvent.key.key))
                    {
                        titleDirty = true;
                        Console.WriteLine(
                            $"{playback.CreateConsoleDiagnostic(jointCount, jointCount)} " +
                            $"aim={(aimEnabled ? "on" : "off")} ik={(ikEnabled ? "on" : "off")}");
                    }
                }

                if (!running)
                    break;

                ulong currentCounter = SDL_GetPerformanceCounter();
                double elapsedSeconds = (currentCounter - previousCounter) / (double)frequency;
                previousCounter = currentCounter;
                playback.Advance(elapsedSeconds);

                AnimationClip frameClip = playback.CurrentClip;
                float frameSampleTime = playback.SampleTime;
                ExecuteInteractiveFrame(frameClip, frameSampleTime, jointCount, () =>
                {
                    SelectedIkAimPose presentedPose = ikAimPresentation.Apply(
                        playback.CreatePose(),
                        aimEnabled,
                        ikEnabled);
                    CharacterAnimationFrame frame = CreateAnimationFrame(
                        skin,
                        presentedPose.Pose,
                        frameSampleTime);
                    UploadPalette(frame.Palette);
                    if (playback.IsSkeletonVisible)
                    {
                        SkeletonDebugGeometry skeleton = SkeletonDebugGeometry.Create(frame.GlobalPose, skeletonAxisLength);
                        UploadSkeleton(skeleton.Vertices);
                    }

                    if (titleDirty || currentCounter - lastTitleCounter >= frequency / 10)
                    {
                        SetWindowTitle(
                            $"{playback.CreateWindowTitle(jointCount, frame.Palette.JointMatrices.Count)} " +
                            $"| Aim={(aimEnabled ? "on" : "off")} IK={(ikEnabled ? "on" : "off")} " +
                            $"error={presentedPose.EndEffectorError:F4}m");
                        lastTitleCounter = currentCounter;
                        titleDirty = false;
                    }

                    SDL_GPUCommandBuffer* command = AcquireCommand();
                    SDL_GPUTexture* swapchain;
                    uint swapchainWidth;
                    uint swapchainHeight;
                    if (!SDL_WaitAndAcquireGPUSwapchainTexture(command, window, &swapchain, &swapchainWidth, &swapchainHeight))
                        throw new InvalidOperationException($"SDL GPU swapchain acquisition failed: {SDL_GetError()}");
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
                        throw new InvalidOperationException($"SDL GPU visible submission failed: {SDL_GetError()}");
                });
                SDL_Delay(16);
            }
        }

        public void Dispose()
        {
            if (device is not null)
                _ = SDL_WaitForGPUIdle(device);
            ReleaseTexture(ref visibleDepth);
            ReleaseTexture(ref offscreenDepth);
            ReleaseTexture(ref offscreenColor);
            if (skeletonTransferBuffer is not null && device is not null)
                SDL_ReleaseGPUTransferBuffer(device, skeletonTransferBuffer);
            skeletonTransferBuffer = null;
            ReleaseBuffer(ref skeletonVertexBuffer);
            characterPalette?.Dispose();
            characterPalette = null;
            attachmentMesh?.Dispose();
            attachmentMesh = null;
            attachmentRenderer?.Dispose();
            attachmentRenderer = null;
            characterMesh?.Dispose();
            characterMesh = null;
            characterRenderer?.Dispose();
            characterRenderer = null;
            if (skeletonPipeline is not null && device is not null)
                SDL_ReleaseGPUGraphicsPipeline(device, skeletonPipeline);
            skeletonPipeline = null;
            if (skeletonFragmentShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, skeletonFragmentShader);
            skeletonFragmentShader = null;
            if (skeletonVertexShader is not null && device is not null)
                SDL_ReleaseGPUShader(device, skeletonVertexShader);
            skeletonVertexShader = null;
            if (device is not null)
            {
                if (windowClaimed && window is not null)
                {
                    SDL_ReleaseWindowFromGPUDevice(device, window);
                    windowClaimed = false;
                }
                SDL_DestroyGPUDevice(device);
            }
            device = null;
            if (window is not null)
                SDL_DestroyWindow(window);
            window = null;
            SDL_Quit();
        }

        private static bool ApplyControl(
            CharacterPlaybackController playback,
            SkeletonJointMask upperBodyMask,
            ref bool aimEnabled,
            ref bool ikEnabled,
            SDL_Keycode key)
        {
            switch (key)
            {
                case SDL_Keycode.SDLK_LEFT:
                    playback.SelectPrevious();
                    return true;
                case SDL_Keycode.SDLK_RIGHT:
                    playback.SelectNext();
                    return true;
                case SDL_Keycode.SDLK_1:
                    playback.RequestLocomotion("Idle_Loop");
                    return true;
                case SDL_Keycode.SDLK_2:
                    playback.RequestLocomotion("Walk_Loop");
                    return true;
                case SDL_Keycode.SDLK_3:
                    playback.SignalAction("Sword_Attack");
                    return true;
                case SDL_Keycode.SDLK_4:
                    playback.SignalLayeredAction("Sword_Attack", upperBodyMask);
                    return true;
                case SDL_Keycode.SDLK_5:
                    aimEnabled = !aimEnabled;
                    return true;
                case SDL_Keycode.SDLK_6:
                    ikEnabled = !ikEnabled;
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

        private void SetWindowTitle(string title)
        {
            byte[] titleBytes = Encoding.UTF8.GetBytes(title + '\0');
            fixed (byte* titlePointer = titleBytes)
            {
                if (!SDL_SetWindowTitle(window, titlePointer))
                    throw new InvalidOperationException($"SDL could not update the diagnostic window title: {SDL_GetError()}");
            }
        }

        private void Render(
            SDL_GPUCommandBuffer* command,
            SDL_GPUTexture* color,
            SDL_GPUTexture* depth,
            uint renderWidth,
            uint renderHeight,
            Matrix4x4 viewProjection,
            bool includeSkeleton,
            Matrix4x4? attachmentWorld = null,
            Matrix4x4? characterWorld = null)
        {
            var colorTarget = new SDL_GPUColorTargetInfo
            {
                texture = color,
                clear_color = ClearColor,
                load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
                store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
            };
            var depthTarget = new SDL_GPUDepthStencilTargetInfo
            {
                texture = depth,
                clear_depth = 1.0f,
                load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
                store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE,
                stencil_load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_DONT_CARE,
                stencil_store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_DONT_CARE,
            };
            SDL_GPURenderPass* pass = SDL_BeginGPURenderPass(command, &colorTarget, 1, &depthTarget);
            if (pass is null)
                throw new InvalidOperationException($"SDL GPU render pass failed: {SDL_GetError()}");

            Matrix4x4 characterTransform = characterWorld ?? Matrix4x4.Identity;

            for (int sectionIndex = 0; sectionIndex < characterMesh!.SectionCount; sectionIndex++)
            {
                Vector4 colorValue = sectionIndex % 2 == 0
                    ? new Vector4(0.95f, 0.28f, 0.18f, 1.0f)
                    : new Vector4(0.16f, 0.62f, 0.98f, 1.0f);
                characterRenderer!.DrawSection(
                    command,
                    pass,
                    characterMesh,
                    characterPalette!,
                    sectionIndex,
                    new SkinnedCharacterDraw(
                        characterTransform,
                        viewProjection,
                        colorValue,
                        new Vector3(-0.35f, -0.70f, -0.62f)));
            }

            if (attachmentWorld is Matrix4x4 bowWorld)
            {
                if (attachmentRenderer is null || attachmentMesh is null)
                    throw new InvalidOperationException("The character GPU session has no uploaded static attachment.");
                attachmentRenderer.Draw(
                    command,
                    pass,
                    attachmentMesh,
                    new StaticMeshDraw(
                        bowWorld,
                        viewProjection,
                        new Vector3(0.90f, 0.65f, 0.12f),
                        new Vector3(-0.35f, -0.70f, -0.62f)));
            }

            if (includeSkeleton)
            {
                SDL_BindGPUGraphicsPipeline(pass, skeletonPipeline);
                var skeletonBinding = new SDL_GPUBufferBinding { buffer = skeletonVertexBuffer };
                SDL_BindGPUVertexBuffers(pass, 0, &skeletonBinding, 1);
                Matrix4x4 transposedViewProjection = Matrix4x4.Transpose(viewProjection);
                SDL_PushGPUVertexUniformData(command, 0, (IntPtr)(&transposedViewProjection), (uint)sizeof(Matrix4x4));
                SDL_DrawGPUPrimitives(pass, skeletonVertexCount, 1, 0, 0);
            }

            SDL_EndGPURenderPass(pass);
        }

        private SDL_GPUShader* LoadShader(string name, SDL_GPUShaderStage stage, uint storageBuffers, uint uniformBuffers)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "shaders", ShaderAssetSelector.GetFileName(name, ShaderFormat));
            if (!File.Exists(path))
                throw new FileNotFoundException($"SDL GPU shader asset was not found: {path}", path);
            byte[] code = File.ReadAllBytes(path);
            byte[] entrypointBytes = Encoding.UTF8.GetBytes(ShaderAssetSelector.GetEntrypoint(ShaderFormat) + '\0');
            fixed (byte* codePointer = code)
            fixed (byte* entrypoint = entrypointBytes)
            {
                var info = new SDL_GPUShaderCreateInfo
                {
                    code_size = (nuint)code.Length,
                    code = codePointer,
                    entrypoint = entrypoint,
                    format = ShaderFormat,
                    stage = stage,
                    num_storage_buffers = storageBuffers,
                    num_uniform_buffers = uniformBuffers,
                };
                SDL_GPUShader* shader = SDL_CreateGPUShader(device, &info);
                if (shader is null)
                    throw new InvalidOperationException($"SDL GPU shader creation failed for {path}: {SDL_GetError()}");
                return shader;
            }
        }

        private SdlGpuSkinnedShaderSet LoadCharacterShaders()
        {
            string vertexPath = Path.Combine(
                AppContext.BaseDirectory,
                "shaders",
                ShaderAssetSelector.GetFileName("skinned-character.vert", ShaderFormat));
            string fragmentPath = Path.Combine(
                AppContext.BaseDirectory,
                "shaders",
                ShaderAssetSelector.GetFileName("skinned-character.frag", ShaderFormat));
            if (!File.Exists(vertexPath))
                throw new FileNotFoundException($"SDL GPU shader asset was not found: {vertexPath}", vertexPath);
            if (!File.Exists(fragmentPath))
                throw new FileNotFoundException($"SDL GPU shader asset was not found: {fragmentPath}", fragmentPath);
            return new SdlGpuSkinnedShaderSet(
                ShaderFormat,
                File.ReadAllBytes(vertexPath),
                File.ReadAllBytes(fragmentPath),
                ShaderAssetSelector.GetEntrypoint(ShaderFormat));
        }

        private SdlGpuStaticShaderSet LoadStaticShaders()
        {
            string vertexPath = Path.Combine(
                AppContext.BaseDirectory,
                "shaders",
                ShaderAssetSelector.GetFileName("static-mesh.vert", ShaderFormat));
            string fragmentPath = Path.Combine(
                AppContext.BaseDirectory,
                "shaders",
                ShaderAssetSelector.GetFileName("static-mesh.frag", ShaderFormat));
            if (!File.Exists(vertexPath))
                throw new FileNotFoundException($"SDL GPU static vertex shader asset was not found: {vertexPath}", vertexPath);
            if (!File.Exists(fragmentPath))
                throw new FileNotFoundException($"SDL GPU static fragment shader asset was not found: {fragmentPath}", fragmentPath);
            return new SdlGpuStaticShaderSet(
                ShaderFormat,
                File.ReadAllBytes(vertexPath),
                File.ReadAllBytes(fragmentPath),
                ShaderAssetSelector.GetEntrypoint(ShaderFormat));
        }

        private SDL_GPUGraphicsPipeline* CreateSkeletonPipeline(SDL_GPUTextureFormat colorFormat)
        {
            var vertexBufferDescription = new SDL_GPUVertexBufferDescription
            {
                slot = 0,
                pitch = GpuDebugLineVertex.Stride,
                input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
            };
            SDL_GPUVertexAttribute* attributes = stackalloc SDL_GPUVertexAttribute[2];
            attributes[0] = new SDL_GPUVertexAttribute
            {
                location = 0,
                buffer_slot = 0,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3,
                offset = GpuDebugLineVertex.PositionOffset,
            };
            attributes[1] = new SDL_GPUVertexAttribute
            {
                location = 1,
                buffer_slot = 0,
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT4,
                offset = GpuDebugLineVertex.ColorOffset,
            };
            var colorDescription = new SDL_GPUColorTargetDescription { format = colorFormat };
            var info = new SDL_GPUGraphicsPipelineCreateInfo
            {
                vertex_shader = skeletonVertexShader,
                fragment_shader = skeletonFragmentShader,
                vertex_input_state = new SDL_GPUVertexInputState
                {
                    vertex_buffer_descriptions = &vertexBufferDescription,
                    num_vertex_buffers = 1,
                    vertex_attributes = attributes,
                    num_vertex_attributes = 2,
                },
                primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_LINELIST,
                rasterizer_state = new SDL_GPURasterizerState
                {
                    fill_mode = SDL_GPUFillMode.SDL_GPU_FILLMODE_FILL,
                    cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_NONE,
                    front_face = SDL_GPUFrontFace.SDL_GPU_FRONTFACE_COUNTER_CLOCKWISE,
                },
                multisample_state = new SDL_GPUMultisampleState { sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1 },
                depth_stencil_state = new SDL_GPUDepthStencilState
                {
                    compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_ALWAYS,
                    enable_depth_test = false,
                    enable_depth_write = false,
                },
                target_info = new SDL_GPUGraphicsPipelineTargetInfo
                {
                    color_target_descriptions = &colorDescription,
                    num_color_targets = 1,
                    depth_stencil_format = DepthFormat,
                    has_depth_stencil_target = true,
                },
            };
            SDL_GPUGraphicsPipeline* created = SDL_CreateGPUGraphicsPipeline(device, &info);
            if (created is null)
                throw new InvalidOperationException($"SDL GPU skeleton pipeline creation failed: {SDL_GetError()}");
            return created;
        }

        private void UploadCycled<T>(
            SDL_GPUTransferBuffer* transfer,
            SDL_GPUBuffer* destination,
            T[] values,
            string label)
            where T : unmanaged
        {
            uint byteCount = checked((uint)(values.Length * sizeof(T)));
            IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: true);
            if (mapped == IntPtr.Zero)
                throw new InvalidOperationException($"SDL GPU {label} upload mapping failed: {SDL_GetError()}");
            fixed (T* source = values)
                Buffer.MemoryCopy(source, (void*)mapped, byteCount, byteCount);
            SDL_UnmapGPUTransferBuffer(device, transfer);

            SDL_GPUCommandBuffer* command = AcquireCommand();
            SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
            if (copy is null)
                throw new InvalidOperationException($"SDL GPU {label} upload copy pass failed: {SDL_GetError()}");
            var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = transfer };
            var region = new SDL_GPUBufferRegion { buffer = destination, size = byteCount };
            SDL_UploadToGPUBuffer(copy, &sourceLocation, &region, cycle: true);
            SDL_EndGPUCopyPass(copy);
            if (!SDL_SubmitGPUCommandBuffer(command))
                throw new InvalidOperationException($"SDL GPU {label} upload submission failed: {SDL_GetError()}");
        }

        private void UploadBuffer<T>(SDL_GPUBuffer* destination, T[] values) where T : unmanaged
        {
            uint byteCount = checked((uint)(values.Length * sizeof(T)));
            SDL_GPUTransferBuffer* transfer = CreateTransfer(SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD, byteCount);
            try
            {
                IntPtr mapped = SDL_MapGPUTransferBuffer(device, transfer, cycle: false);
                if (mapped == IntPtr.Zero)
                    throw new InvalidOperationException($"SDL GPU upload mapping failed: {SDL_GetError()}");
                fixed (T* source = values)
                    Buffer.MemoryCopy(source, (void*)mapped, byteCount, byteCount);
                SDL_UnmapGPUTransferBuffer(device, transfer);

                SDL_GPUCommandBuffer* command = AcquireCommand();
                SDL_GPUCopyPass* copy = SDL_BeginGPUCopyPass(command);
                if (copy is null)
                    throw new InvalidOperationException($"SDL GPU upload copy pass failed: {SDL_GetError()}");
                var sourceLocation = new SDL_GPUTransferBufferLocation { transfer_buffer = transfer };
                var region = new SDL_GPUBufferRegion { buffer = destination, size = byteCount };
                SDL_UploadToGPUBuffer(copy, &sourceLocation, &region, cycle: false);
                SDL_EndGPUCopyPass(copy);
                if (!SDL_SubmitGPUCommandBuffer(command))
                    throw new InvalidOperationException($"SDL GPU upload submission failed: {SDL_GetError()}");
            }
            finally
            {
                SDL_ReleaseGPUTransferBuffer(device, transfer);
            }
        }

        private SDL_GPUCommandBuffer* AcquireCommand()
        {
            SDL_GPUCommandBuffer* command = SDL_AcquireGPUCommandBuffer(device);
            if (command is null)
                throw new InvalidOperationException($"SDL GPU command acquisition failed: {SDL_GetError()}");
            return command;
        }

        private SDL_GPUBuffer* CreateBuffer(SDL_GPUBufferUsageFlags usage, uint size)
        {
            var info = new SDL_GPUBufferCreateInfo { usage = usage, size = size };
            SDL_GPUBuffer* buffer = SDL_CreateGPUBuffer(device, &info);
            if (buffer is null)
                throw new InvalidOperationException($"SDL GPU buffer creation failed: {SDL_GetError()}");
            return buffer;
        }

        private SDL_GPUTransferBuffer* CreateTransfer(SDL_GPUTransferBufferUsage usage, uint size)
        {
            var info = new SDL_GPUTransferBufferCreateInfo { usage = usage, size = size };
            SDL_GPUTransferBuffer* transfer = SDL_CreateGPUTransferBuffer(device, &info);
            if (transfer is null)
                throw new InvalidOperationException($"SDL GPU transfer creation failed: {SDL_GetError()}");
            return transfer;
        }

        private SDL_GPUTexture* CreateTexture(SDL_GPUTextureFormat format, SDL_GPUTextureUsageFlags usage, uint textureWidth, uint textureHeight)
        {
            var info = new SDL_GPUTextureCreateInfo
            {
                type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
                format = format,
                usage = usage,
                width = textureWidth,
                height = textureHeight,
                layer_count_or_depth = 1,
                num_levels = 1,
                sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
            };
            SDL_GPUTexture* texture = SDL_CreateGPUTexture(device, &info);
            if (texture is null)
                throw new InvalidOperationException($"SDL GPU texture creation failed: {SDL_GetError()}");
            return texture;
        }

        private void EnsureVisibleDepth(uint textureWidth, uint textureHeight)
        {
            if (visibleDepth is not null && visibleDepthWidth == textureWidth && visibleDepthHeight == textureHeight)
                return;
            ReleaseTexture(ref visibleDepth);
            visibleDepthWidth = textureWidth;
            visibleDepthHeight = textureHeight;
            visibleDepth = CreateTexture(DepthFormat, SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET, textureWidth, textureHeight);
        }

        private void ReleaseBuffer(ref SDL_GPUBuffer* buffer)
        {
            if (buffer is not null && device is not null)
                SDL_ReleaseGPUBuffer(device, buffer);
            buffer = null;
        }

        private void ReleaseTexture(ref SDL_GPUTexture* texture)
        {
            if (texture is not null && device is not null)
                SDL_ReleaseGPUTexture(device, texture);
            texture = null;
        }

    }
}

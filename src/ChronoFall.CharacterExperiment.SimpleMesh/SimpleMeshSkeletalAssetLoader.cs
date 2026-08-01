using System.Numerics;
using Imported = global::SimpleMesh;

namespace ChronoFall.CharacterExperiment.SimpleMesh;

public static class SimpleMeshSkeletalAssetLoader
{
    private const float IdentityTolerance = 1e-5f;

    public static SkeletalCharacterAsset LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        try
        {
            return MapModel(Imported.Model.FromFile(path), path);
        }
        catch (SkeletalAssetLoadException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new SkeletalAssetLoadException(
                path,
                $"SimpleMesh import failed: {exception.Message}",
                innerException: exception);
        }
    }

    internal static SkeletalCharacterAsset MapModel(Imported.Model model, string source)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        NodeGraph graph = BuildNodeGraph(model, source);
        if (model.Skins.Length != 1)
            throw Error(source, $"Expected one skin, but found {model.Skins.Length}.");

        Imported.Skin skin = model.Skins[0] ?? throw Error(source, "Skin 0 is null.");
        if (skin.Bones.Length == 0)
            throw Error(source, "The skin contains no joints.");
        if (skin.InverseBindMatrices.Length != skin.Bones.Length)
        {
            throw Error(
                source,
                $"The skin has {skin.Bones.Length} joints but {skin.InverseBindMatrices.Length} inverse-bind matrices.");
        }

        Imported.ModelNode[] skinnedNodes = graph.Nodes
            .Where(node => node.Geometry is not null && ReferenceEquals(node.Skin, skin))
            .ToArray();
        if (skinnedNodes.Length != 1)
            throw Error(source, $"Expected one skinned mesh node, but found {skinnedNodes.Length}.");

        Imported.ModelNode meshNode = skinnedNodes[0];
        Imported.ModelNode rootJoint = skin.Bones[0] ?? throw Error(source, "Joint 0 is null.");
        graph.Parents.TryGetValue(meshNode, out Imported.ModelNode? meshParent);
        graph.Parents.TryGetValue(rootJoint, out Imported.ModelNode? rootParent);
        if (meshParent is null || !ReferenceEquals(meshParent, rootParent))
        {
            throw Error(
                source,
                "The skinned mesh and joint hierarchy must be siblings in one model space.",
                targetNode: meshNode.Name,
                channelPath: "geometry");
        }
        if (!IsIdentity(meshParent.Transform) || !IsIdentity(meshNode.Transform))
        {
            throw Error(
                source,
                "The selected mesh and its shared parent must use identity model-space transforms.",
                targetNode: meshNode.Name,
                channelPath: "geometry");
        }

        SkeletonDefinition skeleton = MapSkeleton(skin, graph, source);
        SkinDefinition skinDefinition = MapSkin(skin, skeleton, source);
        SkinnedMeshDefinition mesh = MapMesh(meshNode, skinDefinition, source);
        AnimationClip[] clips = MapAnimations(model.Animations, skeleton, source);
        try
        {
            return new SkeletalCharacterAsset(mesh, clips);
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, innerException: exception);
        }
    }

    private static NodeGraph BuildNodeGraph(Imported.Model model, string source)
    {
        if (model.Roots.Length == 0)
            throw Error(source, "The model contains no scene roots.");

        var nodes = new List<Imported.ModelNode>();
        var parents = new Dictionary<Imported.ModelNode, Imported.ModelNode?>();
        foreach (Imported.ModelNode root in model.Roots)
            Visit(root, null);

        return new NodeGraph(nodes, parents);

        void Visit(Imported.ModelNode? node, Imported.ModelNode? parent)
        {
            if (node is null)
                throw Error(source, "The model hierarchy contains a null node.");
            if (!parents.TryAdd(node, parent))
                throw Error(source, $"Node '{node.Name}' appears more than once in the model hierarchy.");

            nodes.Add(node);
            foreach (Imported.ModelNode child in node.Children)
                Visit(child, node);
        }
    }

    private static SkeletonDefinition MapSkeleton(
        Imported.Skin skin,
        NodeGraph graph,
        string source)
    {
        var jointIndices = new Dictionary<Imported.ModelNode, int>();
        for (int index = 0; index < skin.Bones.Length; index++)
        {
            Imported.ModelNode joint = skin.Bones[index] ?? throw Error(source, $"Joint {index} is null.");
            if (!graph.Parents.ContainsKey(joint))
                throw Error(source, $"Joint {index} ('{joint.Name}') is not reachable from the loaded scene.");
            if (!jointIndices.TryAdd(joint, index))
                throw Error(source, $"Joint {index} ('{joint.Name}') is duplicated in the skin.");
        }

        var joints = new SkeletonJoint[skin.Bones.Length];
        for (int index = 0; index < skin.Bones.Length; index++)
        {
            Imported.ModelNode joint = skin.Bones[index];
            graph.Parents.TryGetValue(joint, out Imported.ModelNode? parent);
            int parentIndex = parent is not null && jointIndices.TryGetValue(parent, out int resolvedParent)
                ? resolvedParent
                : -1;
            if ((index == 0 && parentIndex != -1) || (index > 0 && (parentIndex < 0 || parentIndex >= index)))
            {
                throw Error(
                    source,
                    $"Joint {index} ('{joint.Name}') is not in parent-first hierarchy order.",
                    targetNode: joint.Name,
                    channelPath: "hierarchy");
            }

            if (!Matrix4x4.Decompose(joint.Transform, out Vector3 scale, out Quaternion rotation, out Vector3 translation))
            {
                throw Error(
                    source,
                    "The local bind transform cannot be decomposed into TRS.",
                    targetNode: joint.Name,
                    channelPath: "hierarchy");
            }

            try
            {
                joints[index] = new SkeletonJoint(
                    joint.Name,
                    parentIndex,
                    new JointTransform(translation, rotation, scale));
            }
            catch (Exception exception)
            {
                throw Error(
                    source,
                    exception.Message,
                    targetNode: joint.Name,
                    channelPath: "hierarchy",
                    innerException: exception);
            }
        }

        try
        {
            return new SkeletonDefinition(joints);
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, channelPath: "hierarchy", innerException: exception);
        }
    }

    private static SkinDefinition MapSkin(Imported.Skin skin, SkeletonDefinition skeleton, string source)
    {
        try
        {
            return new SkinDefinition(skeleton, skin.InverseBindMatrices);
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, channelPath: "inverseBindMatrices", innerException: exception);
        }
    }

    private static SkinnedMeshDefinition MapMesh(
        Imported.ModelNode meshNode,
        SkinDefinition skin,
        string source)
    {
        Imported.Geometry geometry = meshNode.Geometry!;
        if (geometry.Kind != Imported.GeometryKind.Triangles)
        {
            throw Error(
                source,
                $"Geometry kind '{geometry.Kind}' is unsupported; indexed triangles are required.",
                targetNode: meshNode.Name,
                channelPath: "geometry");
        }

        const Imported.VertexAttributes required =
            Imported.VertexAttributes.Normal |
            Imported.VertexAttributes.Texture1 |
            Imported.VertexAttributes.Joints;
        if ((geometry.Vertices.Descriptor.Attributes & required) != required)
        {
            throw Error(
                source,
                "Geometry must contain NORMAL, TEXCOORD_0, JOINTS_0, and WEIGHTS_0 data.",
                targetNode: meshNode.Name,
                channelPath: "geometry");
        }

        var vertices = new SkinnedVertex[geometry.Vertices.Count];
        for (int index = 0; index < vertices.Length; index++)
        {
            Imported.Point4<ushort> jointIndices = geometry.Vertices.JointIndices[index];
            try
            {
                vertices[index] = new SkinnedVertex(
                    geometry.Vertices.Position[index],
                    geometry.Vertices.Normal[index],
                    geometry.Vertices.Texture1[index],
                    new SkinInfluences(
                        new JointIndices4(jointIndices.A, jointIndices.B, jointIndices.C, jointIndices.D),
                        geometry.Vertices.JointWeights[index]));
            }
            catch (Exception exception)
            {
                throw Error(
                    source,
                    $"Vertex {index} is invalid: {exception.Message}",
                    targetNode: meshNode.Name,
                    channelPath: "geometry",
                    innerException: exception);
            }
        }

        var indices = new List<uint>(geometry.Indices.Length);
        var sections = new List<SkinnedMeshSection>(geometry.Groups.Length);
        for (int groupIndex = 0; groupIndex < geometry.Groups.Length; groupIndex++)
        {
            Imported.TriangleGroup group = geometry.Groups[groupIndex] ??
                throw Error(
                    source,
                    $"Geometry group {groupIndex} is null.",
                    targetNode: meshNode.Name,
                    channelPath: "geometry");
            if (group.StartIndex < 0 || group.IndexCount <= 0 || group.IndexCount % 3 != 0 ||
                group.StartIndex > geometry.Indices.Length - group.IndexCount)
            {
                throw Error(
                    source,
                    $"Geometry group {groupIndex} has an invalid index range.",
                    targetNode: meshNode.Name,
                    channelPath: "geometry");
            }

            int sectionStart = indices.Count;
            for (int offset = 0; offset < group.IndexCount; offset++)
            {
                uint localIndex = geometry.Indices[group.StartIndex + offset];
                long resolvedIndex = (long)localIndex + group.BaseVertex;
                if (resolvedIndex < 0 || resolvedIndex >= vertices.Length)
                {
                    throw Error(
                        source,
                        $"Geometry group {groupIndex} resolves index {offset} to vertex {resolvedIndex}, outside {vertices.Length} vertices.",
                        targetNode: meshNode.Name,
                        channelPath: "geometry");
                }

                indices.Add((uint)resolvedIndex);
            }

            string materialName = group.Material?.Name ?? string.Empty;
            try
            {
                sections.Add(new SkinnedMeshSection(materialName, sectionStart, group.IndexCount));
            }
            catch (Exception exception)
            {
                throw Error(
                    source,
                    $"Geometry group {groupIndex} is invalid: {exception.Message}",
                    targetNode: meshNode.Name,
                    channelPath: "geometry",
                    innerException: exception);
            }
        }

        try
        {
            string name = string.IsNullOrWhiteSpace(geometry.Name) ? meshNode.Name : geometry.Name;
            return new SkinnedMeshDefinition(name, skin, vertices, indices, sections);
        }
        catch (Exception exception)
        {
            throw Error(
                source,
                exception.Message,
                targetNode: meshNode.Name,
                channelPath: "geometry",
                innerException: exception);
        }
    }

    private static AnimationClip[] MapAnimations(
        Imported.Animation[] animations,
        SkeletonDefinition skeleton,
        string source)
    {
        if (animations.Length == 0)
            throw Error(source, "The model contains no animations.");

        var names = new HashSet<string>(StringComparer.Ordinal);
        var clips = new AnimationClip[animations.Length];
        for (int index = 0; index < animations.Length; index++)
        {
            Imported.Animation animation = animations[index] ??
                throw Error(source, $"Animation {index} is null.");
            if (string.IsNullOrWhiteSpace(animation.Name))
                throw Error(source, $"Animation {index} has no name.");
            if (!names.Add(animation.Name))
                throw Error(source, $"Animation name '{animation.Name}' is duplicated.", animation.Name);

            Dictionary<int, Vector3AnimationChannel> translations = MapTranslations(animation, skeleton, source);
            Dictionary<int, QuaternionAnimationChannel> rotations = MapRotations(animation, skeleton, source);
            Dictionary<int, Vector3AnimationChannel> scales = MapScales(animation, skeleton, source);
            var tracks = new JointAnimationTrack[skeleton.JointCount];
            for (int jointIndex = 0; jointIndex < skeleton.JointCount; jointIndex++)
            {
                string target = skeleton.Joints[jointIndex].Name;
                tracks[jointIndex] = new JointAnimationTrack(
                    jointIndex,
                    RequireChannel(translations, jointIndex, source, animation.Name, target, "translation"),
                    RequireChannel(rotations, jointIndex, source, animation.Name, target, "rotation"),
                    RequireChannel(scales, jointIndex, source, animation.Name, target, "scale"));
            }

            try
            {
                clips[index] = new AnimationClip(animation.Name, skeleton, tracks);
            }
            catch (Exception exception)
            {
                throw Error(source, exception.Message, animation.Name, innerException: exception);
            }
        }

        return clips;
    }

    private static Dictionary<int, Vector3AnimationChannel> MapTranslations(
        Imported.Animation animation,
        SkeletonDefinition skeleton,
        string source)
    {
        var result = new Dictionary<int, Vector3AnimationChannel>();
        foreach (Imported.TranslationChannel? channel in animation.Translations)
        {
            if (channel is null)
                throw Error(source, "Channel is null.", animation.Name, channelPath: "translation");
            int jointIndex = ResolveTarget(skeleton, source, animation.Name, channel.Target, "translation");
            RequireLinear(channel.Interpolation, source, animation.Name, channel.Target, "translation");
            Vector3AnimationChannel mapped = MapVector3Channel(
                channel.Keyframes.Select(keyframe => (keyframe.Time, keyframe.Translation)),
                source,
                animation.Name,
                channel.Target,
                "translation");
            AddUnique(result, jointIndex, mapped, source, animation.Name, channel.Target, "translation");
        }

        return result;
    }

    private static Dictionary<int, QuaternionAnimationChannel> MapRotations(
        Imported.Animation animation,
        SkeletonDefinition skeleton,
        string source)
    {
        var result = new Dictionary<int, QuaternionAnimationChannel>();
        foreach (Imported.RotationChannel? channel in animation.Rotations)
        {
            if (channel is null)
                throw Error(source, "Channel is null.", animation.Name, channelPath: "rotation");
            int jointIndex = ResolveTarget(skeleton, source, animation.Name, channel.Target, "rotation");
            RequireLinear(channel.Interpolation, source, animation.Name, channel.Target, "rotation");
            QuaternionAnimationChannel mapped = MapQuaternionChannel(
                channel.Keyframes.Select(keyframe => (keyframe.Time, keyframe.Rotation)),
                source,
                animation.Name,
                channel.Target,
                "rotation");
            AddUnique(result, jointIndex, mapped, source, animation.Name, channel.Target, "rotation");
        }

        return result;
    }

    private static Dictionary<int, Vector3AnimationChannel> MapScales(
        Imported.Animation animation,
        SkeletonDefinition skeleton,
        string source)
    {
        var result = new Dictionary<int, Vector3AnimationChannel>();
        foreach (Imported.ScaleChannel? channel in animation.Scales)
        {
            if (channel is null)
                throw Error(source, "Channel is null.", animation.Name, channelPath: "scale");
            int jointIndex = ResolveTarget(skeleton, source, animation.Name, channel.Target, "scale");
            RequireLinear(channel.Interpolation, source, animation.Name, channel.Target, "scale");
            Vector3AnimationChannel mapped = MapVector3Channel(
                channel.Keyframes.Select(keyframe => (keyframe.Time, keyframe.Scale)),
                source,
                animation.Name,
                channel.Target,
                "scale");
            AddUnique(result, jointIndex, mapped, source, animation.Name, channel.Target, "scale");
        }

        return result;
    }

    private static int ResolveTarget(
        SkeletonDefinition skeleton,
        string source,
        string clip,
        string target,
        string path)
    {
        if (string.IsNullOrWhiteSpace(target) || !skeleton.TryGetJointIndex(target, out int jointIndex))
            throw Error(source, "Target does not resolve to a skeleton joint.", clip, target, path);
        return jointIndex;
    }

    private static void RequireLinear(
        Imported.AnimationInterpolation interpolation,
        string source,
        string clip,
        string target,
        string path)
    {
        if (interpolation != Imported.AnimationInterpolation.Linear)
        {
            throw Error(
                source,
                $"Interpolation '{interpolation}' is unsupported; M1 accepts only LINEAR.",
                clip,
                target,
                path);
        }
    }

    private static Vector3AnimationChannel MapVector3Channel(
        IEnumerable<(float Time, Vector3 Value)> values,
        string source,
        string clip,
        string target,
        string path)
    {
        try
        {
            return new Vector3AnimationChannel(values.Select(value => new Vector3Keyframe(value.Time, value.Value)));
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, clip, target, path, exception);
        }
    }

    private static QuaternionAnimationChannel MapQuaternionChannel(
        IEnumerable<(float Time, Quaternion Value)> values,
        string source,
        string clip,
        string target,
        string path)
    {
        try
        {
            return new QuaternionAnimationChannel(
                values.Select(value => new QuaternionKeyframe(value.Time, value.Value)));
        }
        catch (Exception exception)
        {
            throw Error(source, exception.Message, clip, target, path, exception);
        }
    }

    private static void AddUnique<T>(
        Dictionary<int, T> channels,
        int jointIndex,
        T channel,
        string source,
        string clip,
        string target,
        string path)
    {
        if (!channels.TryAdd(jointIndex, channel))
            throw Error(source, "A second channel targets the same joint and path.", clip, target, path);
    }

    private static T RequireChannel<T>(
        IReadOnlyDictionary<int, T> channels,
        int jointIndex,
        string source,
        string clip,
        string target,
        string path)
    {
        if (!channels.TryGetValue(jointIndex, out T? channel))
            throw Error(source, "Required channel is missing.", clip, target, path);
        return channel;
    }

    private static bool IsIdentity(Matrix4x4 matrix) =>
        Approximately(matrix.M11, 1.0f) && Approximately(matrix.M12, 0.0f) &&
        Approximately(matrix.M13, 0.0f) && Approximately(matrix.M14, 0.0f) &&
        Approximately(matrix.M21, 0.0f) && Approximately(matrix.M22, 1.0f) &&
        Approximately(matrix.M23, 0.0f) && Approximately(matrix.M24, 0.0f) &&
        Approximately(matrix.M31, 0.0f) && Approximately(matrix.M32, 0.0f) &&
        Approximately(matrix.M33, 1.0f) && Approximately(matrix.M34, 0.0f) &&
        Approximately(matrix.M41, 0.0f) && Approximately(matrix.M42, 0.0f) &&
        Approximately(matrix.M43, 0.0f) && Approximately(matrix.M44, 1.0f);

    private static bool Approximately(float value, float expected) =>
        float.IsFinite(value) && MathF.Abs(value - expected) <= IdentityTolerance;

    private static SkeletalAssetLoadException Error(
        string source,
        string reason,
        string? clipName = null,
        string? targetNode = null,
        string? channelPath = null,
        Exception? innerException = null) =>
        new(source, reason, clipName, targetNode, channelPath, innerException);

    private sealed record NodeGraph(
        IReadOnlyList<Imported.ModelNode> Nodes,
        IReadOnlyDictionary<Imported.ModelNode, Imported.ModelNode?> Parents);
}

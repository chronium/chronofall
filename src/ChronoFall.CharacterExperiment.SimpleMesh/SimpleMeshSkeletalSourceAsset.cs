namespace ChronoFall.CharacterExperiment.SimpleMesh;

public sealed class SimpleMeshSkeletalSourceAsset
{
    internal SimpleMeshSkeletalSourceAsset(
        string meshNodeName,
        string meshName,
        string skinName,
        SkeletalCharacterAsset asset)
    {
        MeshNodeName = meshNodeName ?? throw new ArgumentNullException(nameof(meshNodeName));
        MeshName = meshName ?? throw new ArgumentNullException(nameof(meshName));
        SkinName = skinName ?? throw new ArgumentNullException(nameof(skinName));
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
    }

    public string MeshNodeName { get; }

    public string MeshName { get; }

    public string SkinName { get; }

    public SkeletalCharacterAsset Asset { get; }
}

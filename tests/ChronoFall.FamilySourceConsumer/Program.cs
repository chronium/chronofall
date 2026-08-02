using ChronoFall.CharacterPresentation.Cooking;
using ChronoFall.CharacterPresentation.SdlGpu;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: ChronoFall.FamilySourceConsumer <cooked-character.cfskel>");
    return 2;
}

using FileStream stream = File.OpenRead(args[0]);
CookedSkeletalCharacterAsset cooked = SkeletalAssetCookedFormat.Read(stream);
_ = typeof(SdlGpuSkinnedShaderSet).Assembly;

if (cooked.Descriptor.AssetId != "quaternius-ual1-standard" || cooked.Asset.Animations.Count != 3)
{
    Console.Error.WriteLine("Unexpected shared character content.");
    return 1;
}

Console.WriteLine(
    $"FAMILY_SOURCE_CONSUMER_SUCCESS asset={cooked.Descriptor.AssetId} " +
    $"joints={cooked.Asset.Mesh.Skin.Skeleton.JointCount} clips={cooked.Asset.Animations.Count}");
return 0;

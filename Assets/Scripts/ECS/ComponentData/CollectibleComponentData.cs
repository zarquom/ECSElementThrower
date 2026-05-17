using Unity.Entities;

public struct CollectibleComponentData : IComponentData
{
    public BlobAssetReference<CollectiblePool> CollectiblePoolReference;
}

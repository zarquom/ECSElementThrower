using Unity.Entities;
using Unity.Physics;

public struct CollectiblePool
{
    public BlobArray<CollectibleContainer> CollectibleData;
}
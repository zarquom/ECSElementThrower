using Unity.Entities;

public struct ElementCollectibleComponentData : IComponentData
{
    public int Amount;
    public CollectibleType ElementType;
}

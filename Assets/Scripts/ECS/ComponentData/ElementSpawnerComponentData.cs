using Unity.Entities;

public struct ElementSpawnerComponentData : IComponentData
{
    public int BulletCount;
}

public struct BulletBufferElementData : IBufferElementData
{
    public Entity BulletPrefab;
}
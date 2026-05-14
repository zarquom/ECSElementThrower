using Unity.Entities;

public struct BulletSpawnerComponentData : IComponentData
{
    public int BulletCount;
}

public struct BulletBufferElementData : IBufferElementData
{
    public Entity BulletPrefab;
}
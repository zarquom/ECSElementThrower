using Unity.Entities;
using Unity.Mathematics;

public struct BulletSpawnerComponentData : IComponentData
{
    public Random Instance;
    public int BulletCount;
}

public struct BulletBufferElementData : IBufferElementData
{
    public Entity BulletPrefab;
}
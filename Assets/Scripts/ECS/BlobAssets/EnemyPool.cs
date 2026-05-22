using Unity.Entities;
using Unity.Physics;

public struct EnemyPool
{
    public BlobArray<EnemyContainer> EnemyData;
}
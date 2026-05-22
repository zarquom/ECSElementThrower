using Unity.Entities;

public struct EnemiesComponentData : IComponentData
{
    public BlobAssetReference<EnemyPool> EnemyPoolReference;
}

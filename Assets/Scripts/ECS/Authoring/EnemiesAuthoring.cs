using Data;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EnemiesAuthoring : MonoBehaviour
{
    [SerializeField] private List<EnemySO> _enemiesSO;

    public List<EnemySO> EnemiesSO => _enemiesSO;
}

public class EnemiesBaker : Baker<EnemiesAuthoring>
{
    public override void Bake(EnemiesAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var enemyBlobAsset = CreateEnemyBlobAsset(authoring.EnemiesSO);
        AddBlobAsset(ref enemyBlobAsset, out _);
        AddComponent(entity, new EnemiesComponentData
        {
            EnemyPoolReference = enemyBlobAsset
        });
    }

    private BlobAssetReference<EnemyPool> CreateEnemyBlobAsset(IReadOnlyList<EnemySO> enemyData)
    {
        using var builder = new BlobBuilder(Allocator.Temp);
        ref var enemyPool = ref builder.ConstructRoot<EnemyPool>();
        BlobBuilderArray<EnemyContainer> arrayBuilder = builder.Allocate(ref enemyPool.EnemyData, enemyData.Count);
        for (int i = 0; i < enemyData.Count; i++)
        {
            var enemy = enemyData[i];
            EnemyType enemyType = enemy.Type;
            float damage = enemy.Damage;
            arrayBuilder[i] = new EnemyContainer { Damage = damage, Type = enemyType };
        }

        return builder.CreateBlobAssetReference<EnemyPool>(Allocator.Persistent);
    }
}
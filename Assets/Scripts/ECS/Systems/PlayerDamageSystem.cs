using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PlayerDetectionSystem))]
public partial struct PlayerDamageSystem : ISystem
{
    private EntityQuery _damageEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemiesComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _damageEntityQuery = SystemAPI.QueryBuilder().WithAll<PlayerDamagedComponentData>().Build();
        state.RequireForUpdate(_damageEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> damagedEntities = _damageEntityQuery.ToEntityArray(Allocator.Temp);

        var enemiesComponentData = SystemAPI.GetSingleton<EnemiesComponentData>();
        ref var enemiesPool = ref enemiesComponentData.EnemyPoolReference.Value;
        ref BlobArray<EnemyContainer> enemiesData = ref enemiesPool.EnemyData;

        var ecb = GetEntityCommandBuffer(ref state);

        for (int i = 0; damagedEntities.Length > i; i++)
        {
            ExecuteDamageLogic(ref state, damagedEntities, i, ref enemiesData, ecb);
        }

        damagedEntities.Dispose();
    }

    [BurstCompile]
    private void ExecuteDamageLogic(ref SystemState state, NativeArray<Entity> damagedEntities, int index, ref BlobArray<EnemyContainer> enemiesData, EntityCommandBuffer ecb)
    {
        var damagedEntity = damagedEntities[index];
        var enemyComponentData = SystemAPI.GetComponent<EnemyComponentData>(damagedEntity);

        for (int i = 0; i < enemiesData.Length; i++)
        {
            if (enemiesData[i].Type == enemyComponentData.Type)
            {
                Debug.Log("Player damaged by " + enemiesData[i].Type + " for " + enemiesData[i].Damage + " damage. Destroyed " + damagedEntity.ToString());
                ecb.DestroyEntity(damagedEntity);
                var healthEntity = state.EntityManager.CreateEntity();
                ecb.AddComponent(healthEntity, new HealthComponentData { Value = -enemiesData[i].Damage });
                break;
            }
        }
    }

    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

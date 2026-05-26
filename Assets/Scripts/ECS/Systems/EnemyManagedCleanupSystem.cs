using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyManagedCleanupSystem : ISystem
{
    private EntityQuery _enemyManagedEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _enemyManagedEntityQuery = SystemAPI.QueryBuilder().WithAll<EnemyManagedComponentData>().WithNone<LocalTransform>().Build();
        state.RequireForUpdate(_enemyManagedEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = _enemyManagedEntityQuery.ToEntityArray(Allocator.Temp);
        EntityCommandBuffer ecb = GetEntityCommandBuffer(ref state);
        foreach (Entity entity in enemies)
        {
            var enemyManagedComponentData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(entity);
            Object.Destroy(enemyManagedComponentData.Animator.gameObject);
            ecb.RemoveComponent<EnemyManagedComponentData>(entity);
        }
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
    }
}

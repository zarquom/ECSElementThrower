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
        _damageEntityQuery = SystemAPI.QueryBuilder().WithAll<PlayerDamagedComponentData>().Build();
        state.RequireForUpdate(_damageEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> damagedEntities = _damageEntityQuery.ToEntityArray(Allocator.Temp);
        Debug.Log("Damaged!");
        damagedEntities.Dispose();
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

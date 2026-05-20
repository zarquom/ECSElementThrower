using System.Xml;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerManagedCleanupSystem : ISystem
{
    private EntityQuery _playerManagedEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _playerManagedEntityQuery = SystemAPI.QueryBuilder()
        .WithAll<PlayerManagedComponentData>()
        .WithNone<PlayerComponentData, LocalTransform>()
        .Build();
        state.RequireForUpdate(_playerManagedEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var playerAnimationEntity = _playerManagedEntityQuery.GetSingletonEntity();
        var playerAnimationComponentData = _playerManagedEntityQuery.GetSingleton<PlayerManagedComponentData>();

        Object.Destroy(playerAnimationComponentData.AnimatorData.gameObject);
        GetEntityCommandBuffer(ref state).RemoveComponent<PlayerManagedComponentData>(playerAnimationEntity);
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

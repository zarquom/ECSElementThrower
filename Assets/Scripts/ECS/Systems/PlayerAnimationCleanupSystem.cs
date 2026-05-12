using System.Xml;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerAnimationCleanupSystem : ISystem
{
    private EntityQuery _playerAnimationEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _playerAnimationEntityQuery = SystemAPI.QueryBuilder()
        .WithAll<PlayerAnimationComponentData>()
        .WithNone<PlayerComponentData, LocalTransform>()
        .Build();
        state.RequireForUpdate(_playerAnimationEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerAnimationEntity = _playerAnimationEntityQuery.GetSingletonEntity();
        var playerAnimationComponentData = _playerAnimationEntityQuery.GetSingleton<PlayerAnimationComponentData>();

        Object.Destroy(playerAnimationComponentData.AnimatorData.gameObject);
        GetEntityCommandBuffer(ref state).RemoveComponent<PlayerAnimationComponentData>(playerAnimationEntity);
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

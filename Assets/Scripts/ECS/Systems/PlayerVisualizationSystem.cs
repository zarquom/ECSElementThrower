using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerVisualizationSystem : ISystem
{
    private EntityQuery _playerEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _playerEntityQuery = SystemAPI.QueryBuilder()
            .WithAll<PlayerVisualizationComponentData>()
            .WithNone<PlayerAnimationComponentData>()
            .Build();
        state.RequireForUpdate(_playerEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = _playerEntityQuery.GetSingletonEntity();
        var playerVisualizationComponentData = _playerEntityQuery.GetSingleton<PlayerVisualizationComponentData>();

        var playerVisualizationGameObject = Object.Instantiate(playerVisualizationComponentData.PlayerVisualization);

        var ecb = GetEntityCommandBuffer(ref state);
        ecb.AddComponent(playerEntity, new PlayerAnimationComponentData
        {
            AnimatorData = playerVisualizationGameObject.GetComponent<Animator>()
        });
    }

    [BurstCompile]
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

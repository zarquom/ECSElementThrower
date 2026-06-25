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
            .WithNone<PlayerManagedComponentData>()
            .Build();
        state.RequireForUpdate(_playerEntityQuery);
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = _playerEntityQuery.GetSingletonEntity();
        var playerVisualizationComponentData = _playerEntityQuery.GetSingleton<PlayerVisualizationComponentData>();

        var playerVisualizationGameObject = Object.Instantiate(playerVisualizationComponentData.PlayerVisualization);
        state.EntityManager.AddComponentObject(playerEntity, new PlayerManagedComponentData
        {
            AnimatorData = playerVisualizationGameObject.GetComponent<Animator>(),
            GameObjectData = playerVisualizationGameObject.gameObject,
            TransformData = playerVisualizationGameObject.GetComponentInChildren<PlayerVisualizationDownComponent>()
        });
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

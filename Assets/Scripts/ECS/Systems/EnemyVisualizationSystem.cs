using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyVisualizationSystem : ISystem
{
    private EntityQuery _enemyEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _enemyEntityQuery = SystemAPI.QueryBuilder().WithAll<EnemyVisualizationComponentData>().WithNone<EnemyManagedComponentData>().Build();
        state.RequireForUpdate(_enemyEntityQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = _enemyEntityQuery.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in enemies)
        {
            var visualizacionComponentData = state.EntityManager.GetComponentObject<EnemyVisualizationComponentData>(entity);
            var visualizationGameObject = Object.Instantiate(visualizacionComponentData.EnemyVisualization);
            visualizationGameObject.transform.position = SystemAPI.GetComponent<LocalTransform>(entity).Position;
            state.EntityManager.AddComponentObject(entity, new EnemyManagedComponentData
            {
                GameObject = visualizationGameObject,
                Transform = visualizationGameObject.transform,
                Animator = visualizationGameObject.GetComponent<Animator>()
            });
        }
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

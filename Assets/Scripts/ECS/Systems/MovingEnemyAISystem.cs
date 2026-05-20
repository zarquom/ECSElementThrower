using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct MovingEnemyAISystem : ISystem
{
    private EntityQuery _enemyEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _enemyEntityQuery = SystemAPI.QueryBuilder().WithAll<EnemyManagedComponentData>().WithAll<MovingEnemyComponentData>().Build();
        state.RequireForUpdate(_enemyEntityQuery);
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemies = _enemyEntityQuery.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in enemies)
        {
            var enemyManagedComponentData = state.EntityManager.GetComponentObject<EnemyManagedComponentData>(entity);
        }
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

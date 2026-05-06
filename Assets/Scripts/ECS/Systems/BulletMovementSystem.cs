
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    private EntityQuery _bulletEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _bulletEntityQuery = SystemAPI.QueryBuilder().WithAll<BulletComponentData>().WithAll<LocalTransform>().Build();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> bulletEntities = _bulletEntityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> localTransforms = _bulletEntityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        float deltaTime = SystemAPI.Time.DeltaTime;
        for (int i = 0; i < bulletEntities.Length; i++){
            Entity bulletEntity = bulletEntities[i];
            LocalTransform localTransform = localTransforms[i];
            BulletAspect bulletAspect = SystemAPI.GetAspect<BulletAspect>(bulletEntity);

            float3 newBulletPosition = localTransform.Position + bulletAspect.BulletDirection * bulletAspect.BulletSpeed * deltaTime;
            LocalTransform updatedTransform = LocalTransform.FromPositionRotationScale(newBulletPosition,localTransform.Rotation, localTransform.Scale);

            // Apply the updated transform back to the entity
            state.EntityManager.SetComponentData(bulletEntity, updatedTransform);
        }
        bulletEntities.Dispose();
        localTransforms.Dispose();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

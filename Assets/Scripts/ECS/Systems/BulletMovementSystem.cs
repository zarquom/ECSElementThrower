
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
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
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _bulletEntityQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>().WithAll<BulletComponentData>().Build();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        new BulletMovementJob
        {
            DeltaTime = deltaTime,
            Ecb = GetEntityCommandBuffer(ref state)
        }.Schedule(_bulletEntityQuery);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
    }
}

[BurstCompile]
public partial struct BulletMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DeltaTime;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int chunkIndexinQuery, in Entity bulletEntity, in LocalTransform localTransform, BulletComponentData bulletComponentData)
    {
            float3 newBulletPosition = localTransform.Position + bulletComponentData.BulletDirection * bulletComponentData.BulletSpeed * DeltaTime;
            LocalTransform updatedTransform = LocalTransform.FromPositionRotationScale(newBulletPosition, localTransform.Rotation, localTransform.Scale);

            // Apply the updated transform back to the entity
            Ecb.SetComponent(chunkIndexinQuery, bulletEntity, updatedTransform);
    }
}
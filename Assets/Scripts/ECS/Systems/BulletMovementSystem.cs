using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.LowLevel;
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
        state.RequireForUpdate<BulletComponentData>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var jobHandle = new BulletMovementJob
        {
            DeltaTime = deltaTime,
            Ecb = GetEntityCommandBuffer(ref state)
        }.Schedule(state.Dependency);
        jobHandle.Complete();
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
    private void Execute([ChunkIndexInQuery] int chunkIndexinQuery, in Entity bulletEntity, ref PhysicsVelocity bulletVelocity, in LocalTransform localTransform, BulletComponentData bulletComponentData)
    {
        Debug.Log($"Direction: {bulletComponentData.BulletDirection}, Speed: {bulletComponentData.BulletSpeed}");
        float3 linearVelocity = new float3(bulletComponentData.BulletSpeed * bulletComponentData.BulletDirection.x, bulletVelocity.Linear.y, bulletVelocity.Linear.z);

        Ecb.SetComponent(chunkIndexinQuery,bulletEntity, new PhysicsVelocity
        {
            Linear = linearVelocity
        });
    }
}
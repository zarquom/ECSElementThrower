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
public partial struct ElementMovementSystem : ISystem
{
    private EntityQuery _elementEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        _elementEntityQuery = SystemAPI.QueryBuilder().WithAll<ElementComponentData, LocalTransform, PhysicsVelocity>().Build();
        state.RequireForUpdate(_elementEntityQuery);
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var jobHandle = new ElementMovementJob
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
public partial struct ElementMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DeltaTime;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int chunkIndexinQuery, in Entity elementEntity, ref PhysicsVelocity elementVelocity, in LocalTransform localTransform, ElementComponentData elementComponentData)
    {
        float3 linearVelocity = new float3(elementComponentData.BulletSpeed * elementComponentData.BulletDirection.x, elementVelocity.Linear.y, elementVelocity.Linear.z);

        Ecb.SetComponent(chunkIndexinQuery,elementEntity, new PhysicsVelocity
        {
            Linear = linearVelocity
        });
    }
}
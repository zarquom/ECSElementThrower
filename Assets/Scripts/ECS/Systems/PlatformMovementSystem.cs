using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlatformMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        JobHandle job = new PlatformMovementJob
        {
            EcbParallelWriter = GetEntityCommandBuffer(ref state),
            DeltaTime = deltaTime
        }.ScheduleParallel(state.Dependency);
        job.Complete();
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
public partial struct PlatformMovementJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter EcbParallelWriter;
    public float DeltaTime;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, ref LocalTransform localTransform, ref PlatformMovementComponentData platformMovementComponentData)
    {
        float3 initialPosition = platformMovementComponentData.InitialPosition;
        float3 movementVector = platformMovementComponentData.MovementVector;
        float movementSpeed = platformMovementComponentData.MovementSpeed;
        int movementDirection = platformMovementComponentData.IsReverseMovement ? -1 : 1;
        float3 targetPosition = initialPosition + movementVector * movementDirection;
        float3 movementStep = movementSpeed * movementDirection * math.normalize(movementVector) * DeltaTime;
        float3 newPosition = localTransform.Position + movementStep;
        bool shouldReverseMovement = platformMovementComponentData.IsReverseMovement ? math.all(newPosition <= targetPosition) : math.all(newPosition >= targetPosition);
        if (shouldReverseMovement)
        {
            ReverseMovement(sortKey, entity, ref platformMovementComponentData);
        }
        localTransform.Position = newPosition;
        EcbParallelWriter.SetComponent(sortKey, entity, localTransform);
    }
    [BurstCompile]
    private void ReverseMovement(int sortKey, Entity entity, ref PlatformMovementComponentData platformMovementComponentData)
    {
        platformMovementComponentData.IsReverseMovement = !platformMovementComponentData.IsReverseMovement;
        EcbParallelWriter.SetComponent(sortKey, entity, platformMovementComponentData);
    }
}
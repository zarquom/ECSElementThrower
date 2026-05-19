using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.LowLevel;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct SnapPlayerToPlatformSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<PlayerMovementComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerComponentData = SystemAPI.GetSingleton<PlayerComponentData>();
        if (playerComponentData.IsDead) 
            return;

        var playerMovementComponentData = SystemAPI.GetSingleton<PlayerMovementComponentData>();
        if(!playerMovementComponentData.IsGrounded)
            return;
        var groundHitentity = playerMovementComponentData.GroundHitEntity;
        bool isMovingPlatform = SystemAPI.HasComponent<PlatformMovementComponentData>(groundHitentity);
        if (!isMovingPlatform)
            return;
        var movingPlatformComponent = SystemAPI.GetComponent<PlatformMovementComponentData>(groundHitentity);
        JobHandle jobHandle = new SnapToPlatformJob { 
         Ecb = GetEntityCommandBuffer(ref state),
            PlatformMovementComponent = movingPlatformComponent
        }.Schedule(state.Dependency);
        jobHandle.Complete();
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

[BurstCompile]
public partial struct SnapToPlatformJob : IJobEntity
{
    public EntityCommandBuffer Ecb;
    public PlatformMovementComponentData PlatformMovementComponent;
    [BurstCompile]
    private void Execute(in Entity playerEntity, ref PhysicsVelocity playerVelocity, ref PlayerMovementComponentData playerMovement)
    {
        if (playerMovement.Direction != 0)
            return;
        float3 direction = math.normalize(PlatformMovementComponent.MovementVector);
        if (PlatformMovementComponent.IsReverseMovement)
            direction = -direction;
        float platformSpeed = direction.x * PlatformMovementComponent.MovementSpeed;
        float3 linearVelocity = new float3(platformSpeed, playerVelocity.Linear.y, playerVelocity.Linear.z); //Will cause drift as the platform moves modifying local transform
        playerVelocity.Linear = linearVelocity;
        Ecb.SetComponent(playerEntity, playerVelocity);
    }
}
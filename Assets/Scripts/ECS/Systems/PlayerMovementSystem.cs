using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerMovementComponentData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerMovementComponentData = SystemAPI.GetSingleton<PlayerMovementComponentData>();
        var jobHandle = new PlayerMovementJob
        {
            Ecb = GetEntityCommandBuffer(ref state)
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
public partial struct PlayerMovementJob : IJobEntity
{
    public EntityCommandBuffer Ecb;
    private void Execute(Entity player, in LocalTransform localTransform, ref PhysicsVelocity playerVelocity, in PhysicsMass mass, in PlayerComponentData playerComp, ref PlayerMovementComponentData playerMovement)
    {
        if (!playerMovement.IsGrounded)
        {
            return;
        }
        if (playerMovement.IsJump)
        {
            HandleJump(ref playerVelocity, mass, player, playerComp, ref playerMovement);
            return;
        }
        if (playerMovement.Direction == 0f)
        {
            return;
        }

        SetRotation(player, localTransform, playerComp, playerMovement);
        SetVelocity(player, playerVelocity, playerComp, playerMovement);
    }

    [BurstCompile]
    private void HandleJump(ref PhysicsVelocity velocity, PhysicsMass mass, Entity player, PlayerComponentData playerComp, ref PlayerMovementComponentData playerMovement)
    {
        velocity.ApplyLinearImpulse(mass, playerComp.JumpForce * math.up());
        playerMovement.IsJump = false;
    }

    [BurstCompile]
    private void SetRotation(Entity player, LocalTransform localTransform, PlayerComponentData playerComp, PlayerMovementComponentData playerMovement)
    {
        quaternion newRotation = playerMovement.Direction > 0f ? new quaternion(x: 0, y: 0, z: 0, w: 1) : new quaternion(x: 0, y: 1, z: 0, w: 0);
        Ecb.SetComponent(player, new LocalTransform
        {
            Position = localTransform.Position,
            Rotation = newRotation,
            Scale = localTransform.Scale
        });
    }
    [BurstCompile]
    private void SetVelocity(Entity player, PhysicsVelocity playerVelocity, PlayerComponentData playerComp, PlayerMovementComponentData playerMovement)
    {
        float3 linearVelocity = new float3(playerComp.Speed * playerMovement.Direction, playerVelocity.Linear.y, playerVelocity.Linear.z);
        Ecb.SetComponent(player, new PhysicsVelocity
        {
            Linear = linearVelocity
        });
    }
}

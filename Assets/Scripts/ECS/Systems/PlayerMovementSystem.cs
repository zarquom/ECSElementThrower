using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new PlayerMovementJob
        {
            Ecb = GetEntityCommandBuffer(ref state),
            DeltaTime = SystemAPI.Time.DeltaTime
        }.Schedule();
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
    public float DeltaTime;
    private void Execute(Entity player, in LocalTransform localTransform, in PlayerComponentData playerComp, in PlayerMovementComponentData playerMovement)
    {
        if (playerMovement.Direction == 0f)
        {
            return;
        }

        float3 newPosition = localTransform.Position;
        newPosition.x += playerMovement.Direction * playerComp.Speed * DeltaTime;
        quaternion newRotation = playerMovement.Direction > 0f ? new quaternion(x: 0, y: 0, z: 0, w: 1) : new quaternion(x: 0, y: 1, z: 0, w: 0);
        Ecb.SetComponent(player, new LocalTransform
        {
            Position = newPosition,
            Rotation = newRotation,
            Scale = localTransform.Scale
        });
    }
}

using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerThrowSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        // Initialization logic if needed
        state.RequireForUpdate< ElementSpawnerComponentData>();
        state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<PlayerMovementComponentData>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        PlayerComponentData playerComponentData = SystemAPI.GetComponent<PlayerComponentData>(playerEntity);
        PlayerMovementComponentData playerMovementComponentData = SystemAPI.GetComponent<PlayerMovementComponentData>(playerEntity);
        Entity bulletSpawnerEntity = SystemAPI.GetSingletonEntity<ElementSpawnerComponentData>();
        var bulletBufferElementData = state.EntityManager.GetBuffer<BulletBufferElementData>(bulletSpawnerEntity);
        if (!playerComponentData.Throwing)
        {
            return;
        }
        LocalTransform playerTransform= SystemAPI.GetComponent<LocalTransform>(playerEntity);
        playerComponentData.Throwing = false;
        state.EntityManager.SetComponentData(playerEntity, playerComponentData);
        float3 randomPos = playerTransform.Position;
        Entity bulletEntity = state.EntityManager.Instantiate(bulletBufferElementData[0].BulletPrefab);
        ElementComponentData bulletComponentData = SystemAPI.GetComponent<ElementComponentData>(bulletEntity);
        bulletComponentData.BulletDirection = playerMovementComponentData.LastDirection;
        LocalTransform localTransform = LocalTransform.FromPositionRotationScale(randomPos, quaternion.Euler(bulletComponentData.BulletRotation, math.RotationOrder.XYZ), 0.2f);
        state.EntityManager.SetComponentData(bulletEntity, localTransform);
        state.EntityManager.SetComponentData(bulletEntity, bulletComponentData);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Cleanup logic if needed
    }
}

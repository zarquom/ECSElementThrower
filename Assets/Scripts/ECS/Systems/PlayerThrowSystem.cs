using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

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
        Entity elementEntity = state.EntityManager.Instantiate(bulletBufferElementData[0].BulletPrefab); // Switch to the element prefab you want to spawn
        ElementComponentData elementComponentData = SystemAPI.GetComponent<ElementComponentData>(elementEntity);
        elementComponentData.BulletDirection = playerMovementComponentData.LastDirection;
        PhysicsVelocity elementPhysicsVelocity = new PhysicsVelocity { Linear = new float3(elementComponentData.BulletSpeed * elementComponentData.BulletDirection.x, elementComponentData.BulletSpeed, 0f), Angular = float3.zero };
        LocalTransform localTransform = LocalTransform.FromPositionRotationScale(randomPos, quaternion.Euler(elementComponentData.BulletRotation, math.RotationOrder.XYZ), 0.2f);
        state.EntityManager.SetComponentData(elementEntity, localTransform);
        state.EntityManager.SetComponentData(elementEntity, elementComponentData);
        state.EntityManager.SetComponentData(elementEntity, elementPhysicsVelocity);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Cleanup logic if needed
    }
}

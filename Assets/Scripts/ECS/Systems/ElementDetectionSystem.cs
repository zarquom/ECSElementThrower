using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct ElementDetectionSystem : ISystem, ISystemStartStop
{
    private float3 _overlapDetectionOffset;
    private CollisionFilter _deadZoneCollisionFilter;
    private CollisionFilter _groundCollisionFilter;
    private CollisionFilter _enemyCollisionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ElementDetectionComponentData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ElementComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity player = SystemAPI.GetSingletonEntity<ElementDetectionComponentData>();
        ElementDetectionComponentData detectionData = SystemAPI.GetComponent<ElementDetectionComponentData>(player);
        _overlapDetectionOffset = detectionData.OverlapDetectionOffset;
        _deadZoneCollisionFilter = detectionData.DeadZoneCollisionFilter;
        _enemyCollisionFilter = detectionData.EnemyCollisionFilter;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<NextLevelComponentData>())
        {
            return;
        }
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        new ElementDetectionJob
        {
            CollisionWorld = collisionWorld,
            OverlapDetectionOffset = _overlapDetectionOffset,
            DeadZoneCollisionFilter = _deadZoneCollisionFilter,
            EnemyCollisionFilter = _enemyCollisionFilter,
            Ecb = GetEntityCommandBuffer(ref state)
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
public partial struct ElementDetectionJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public CollisionWorld CollisionWorld;
    [Unity.Collections.ReadOnly] public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter EnemyCollisionFilter;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    private unsafe void Execute(in Entity elementEntity, ref ElementComponentData elementComponentData, in PhysicsCollider collider, in LocalTransform localTransform)
    {
        var sphereCollider = (Unity.Physics.SphereCollider*)collider.ColliderPtr;
        var sphereGeometry = sphereCollider->Geometry;

        bool isRemoved = CheckSphere(localTransform, sphereGeometry, DeadZoneCollisionFilter);
        if (isRemoved)
        {
            Ecb.SetEnabled(elementEntity, false);
            Debug.Log("Is removed");
            return;
        }
        //HandleGenericDetectionForManyHits<PlayerDamagedComponentData>(localTransform, boxGeometry, EnemyCollisionFilter);

    }
    [BurstCompile]
    private void HandleGenericDetectionForManyHits<T>(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry, CollisionFilter collisionFilter) where T : unmanaged, IComponentData
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool isCOllision = OverlappingBox(localTransform, boxGeometry, ref hits, collisionFilter);
        if (isCOllision)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Ecb.AddComponent(hits[i].Entity, new T());
            }
        }
        hits.Dispose();
    }

    [BurstCompile]
    private bool OverlappingBox(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry, ref NativeList<DistanceHit> hits, CollisionFilter collisionFilter)
    {
        return CollisionWorld.OverlapBox(localTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, collisionFilter);
    }

    [BurstCompile]
    private bool CheckSphere(LocalTransform localTransform, Unity.Physics.SphereGeometry sphereGeometry, CollisionFilter collisionFilter)
    {
        return CollisionWorld.CheckSphere(localTransform.Position + OverlapDetectionOffset, sphereGeometry.Radius, collisionFilter);
    }
}
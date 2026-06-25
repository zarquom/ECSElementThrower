using System.ComponentModel;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct ElementDetectionSystem : ISystem, ISystemStartStop
{
    private float3 _overlapDetectionOffset;
    private CollisionFilter _deadZoneCollisionFilter;
    private CollisionFilter _groundCollisionFilter;
    private CollisionFilter _collectCollisionFilter;
    private CollisionFilter _notcollectCollisionFilter;

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
        _collectCollisionFilter = detectionData.CollectCollisionFilter;
        _notcollectCollisionFilter = detectionData.NotCollectCollisionFilter;
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
            CollectCollisionFilter = _collectCollisionFilter,
            NotCollectCollisionFilter = _notcollectCollisionFilter,
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
    public CollisionFilter CollectCollisionFilter;
    public CollisionFilter NotCollectCollisionFilter;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    private unsafe void Execute(in Entity elementEntity, ref ElementComponentData elementComponentData, in ElementScoreableComponentData scorableComponent, in PhysicsCollider collider, in LocalTransform localTransform)
    {
        var sphereCollider = (Unity.Physics.SphereCollider*)collider.ColliderPtr;
        var sphereGeometry = sphereCollider->Geometry;

        bool isRemoved = CheckSphere(localTransform, sphereGeometry, DeadZoneCollisionFilter);
        if (isRemoved)
        {
            Ecb.SetEnabled(elementEntity, false);
            return;
        }
        bool isInCollectionZone = CheckSphere(localTransform, sphereGeometry, CollectCollisionFilter);
        if (isInCollectionZone)
        {
            var pointsEntity = Ecb.CreateEntity();
            Ecb.AddComponent(pointsEntity, new PointsComponentData
            {
                Points = 1
            });
            Ecb.RemoveComponent<ElementScoreableComponentData>(elementEntity);
        }
    }

    [BurstCompile]
    private bool CheckSphere(LocalTransform localTransform, Unity.Physics.SphereGeometry sphereGeometry, CollisionFilter collisionFilter)
    {
        return CollisionWorld.CheckSphere(localTransform.Position + OverlapDetectionOffset, sphereGeometry.Radius, collisionFilter);
    }

    [BurstCompile]
    private void SetCollisionFilter(PhysicsCollider collider, uint collidesWith)
    {
        Assert.IsTrue(collider.Value.Value.CollisionType == CollisionType.Convex);

        unsafe
        {
            var header = (ConvexCollider*)collider.ColliderPtr;
            var filter = header->GetCollisionFilter();

            filter.CollidesWith = collidesWith;

            header->SetCollisionFilter(filter);
        }
    }
}
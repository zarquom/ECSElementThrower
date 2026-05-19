using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
public partial struct PlayerDetectionSystem : ISystem, ISystemStartStop
{
    private float3 _overlapDetectionOffset;
    private CollisionFilter _deadZoneCollisionFilter;
    private CollisionFilter _groundCollisionFilter;
    private CollisionFilter _endFlagCollisionFilter;
    private CollisionFilter _collectibleCollisionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerDetectionComponentData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<PlayerComponentData>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerDetectionComponentData>();
        PlayerDetectionComponentData detectionData = SystemAPI.GetComponent<PlayerDetectionComponentData>(player);
        _overlapDetectionOffset = detectionData.OverlapDetectionOffset;
        _deadZoneCollisionFilter = detectionData.DeadZoneCollisionFilter;
        _groundCollisionFilter = detectionData.GroundCollisionFilter;
        _endFlagCollisionFilter = detectionData.EndFlagCollisionFilter;
        _collectibleCollisionFilter = detectionData.CollectibleCollisionFilter;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerComponentData = SystemAPI.GetSingleton<PlayerComponentData>();
        if (playerComponentData.IsDead)
        {
            return;
        }
        if (SystemAPI.HasSingleton<NextLevelComponentData>())
        {
            return;
        }
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        new PlayerDetectionJob
        {
            CollisionWorld = collisionWorld,
            OverlapDetectionOffset = _overlapDetectionOffset,
            DeadZoneCollisionFilter = _deadZoneCollisionFilter,
            GroundCollisionFilter = _groundCollisionFilter,
            EndFlagCollisionFilter = _endFlagCollisionFilter,
            CollectibleCollisionFilter = _collectibleCollisionFilter,
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
public partial struct PlayerDetectionJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public CollisionWorld CollisionWorld;
    [Unity.Collections.ReadOnly] public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter GroundCollisionFilter;
    public CollisionFilter EndFlagCollisionFilter;
    public CollisionFilter CollectibleCollisionFilter;
    public EntityCommandBuffer Ecb;

    [BurstCompile]
    private unsafe void Execute(ref PlayerMovementComponentData playerMovementData, ref PlayerComponentData playerComponentData, in PhysicsCollider collider, in LocalTransform localTransform)
    {


        var boxCollider = (Unity.Physics.BoxCollider*)collider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;

        bool isDead = CheckBox(localTransform, boxGeometry, DeadZoneCollisionFilter);
        playerComponentData.IsDead = isDead;
        if (isDead)
        {
            Debug.Log("Is dead");
            return;
        }
        HandleCollectibleDetection(localTransform, boxGeometry);
        HandleGroundDetection(ref playerMovementData, localTransform, boxGeometry);
        HandleEndFlagDetection(localTransform, boxGeometry);

    }
    private void HandleCollectibleDetection(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool hasRechCollectible = OverlappingBox(localTransform, boxGeometry, ref hits, CollectibleCollisionFilter);
        if (hasRechCollectible)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Ecb.AddComponent(hits[i].Entity, new CollectedCollectibleComponentData());
            }
        }
        hits.Dispose();
    }
    private void HandleEndFlagDetection(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool hasReachEndFlag = OverlappingBox(localTransform, boxGeometry, ref hits, EndFlagCollisionFilter);
        if (hasReachEndFlag)
        {
            Ecb.AddComponent(hits[0].Entity, new NextLevelComponentData());
        }
        hits.Dispose();
    }

    private void HandleGroundDetection(ref PlayerMovementComponentData playerMovementData, LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);
        bool isGrounded = OverlappingBox(localTransform, boxGeometry, ref hits, GroundCollisionFilter);
        if (isGrounded)
        {
            playerMovementData.GroundHitEntity = hits[0].Entity;
        }
        playerMovementData.IsGrounded = isGrounded;
        hits.Dispose();
    }

    [BurstCompile]
    private bool OverlappingBox(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry, ref NativeList<DistanceHit> hits, CollisionFilter collisionFilter)
    {
        return CollisionWorld.OverlapBox(localTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, collisionFilter);
    }

    [BurstCompile]
    private bool CheckBox(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry, CollisionFilter collisionFilter)
    {
        return CollisionWorld.CheckBox(localTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, collisionFilter);
    }
}
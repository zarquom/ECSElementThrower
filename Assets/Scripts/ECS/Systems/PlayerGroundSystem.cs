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
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct PlayerGroundSystem : ISystem, ISystemStartStop
{
    private float3 _overlapDetectionOffset;
    private CollisionFilter _deadZoneCollisionFilter;
    private CollisionFilter _groundCollisionFilter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerGroundComponentData>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerGroundComponentData>();
        PlayerGroundComponentData groundData = SystemAPI.GetComponent<PlayerGroundComponentData>(player);
        _overlapDetectionOffset = groundData.OverlapDetectionOffset;
        _deadZoneCollisionFilter = groundData.DeadZoneCollisionFilter;
        _groundCollisionFilter = groundData.GroundCollisionFilter;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        new PlayerGroundJob
        {
            CollisionWorld = collisionWorld,
            OverlapDetectionOffset = _overlapDetectionOffset,
            DeadZoneCollisionFilter = _deadZoneCollisionFilter,
            GroundCollisionFilter = _groundCollisionFilter
        }.Schedule();
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct PlayerGroundJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public CollisionWorld CollisionWorld;
    [Unity.Collections.ReadOnly] public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter GroundCollisionFilter;

    [BurstCompile]
    private unsafe void Execute(ref PlayerMovementComponentData playerMovementData, ref PlayerComponentData playerComponentData, in PhysicsCollider collider, in LocalTransform localTransform)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);

        var boxCollider = (Unity.Physics.BoxCollider*)collider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;
        
        bool isDead = OverlappingBox(localTransform, boxGeometry, ref hits, DeadZoneCollisionFilter);
        playerComponentData.IsDead = isDead;
        if (isDead)
        {
            Debug.Log("Is dead");
            hits.Dispose();
            return;
        }
        bool isGrounded = OverlappingBox(localTransform, boxGeometry, ref hits, GroundCollisionFilter);
        playerMovementData.IsGrounded = isGrounded;
        foreach (var hit in hits)
        {
            Debug.DrawLine(localTransform.Position, hit.Position, Color.magenta, 2f);
        }
        hits.Dispose();
    }

    [BurstCompile]
    private bool OverlappingBox(LocalTransform localTransform, Unity.Physics.BoxGeometry boxGeometry, ref NativeList<DistanceHit> hits, CollisionFilter collisionFilter)
    {
        return CollisionWorld.OverlapBox(localTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, collisionFilter);
    }
}
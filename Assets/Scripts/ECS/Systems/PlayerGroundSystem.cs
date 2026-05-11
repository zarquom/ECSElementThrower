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
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct PlayerGroundSystem : ISystem, ISystemStartStop
{
    private float3 _overlapDetectionOffset;

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
            OverlapDetectionOffset = _overlapDetectionOffset
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

    [BurstCompile]
    private unsafe void Execute(ref PlayerMovementComponentData playerMovementData, in PhysicsCollider collider, in LocalTransform localTransform)
    {
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.TempJob);

        var boxCollider = (Unity.Physics.BoxCollider*)collider.ColliderPtr;
        var boxGeometry = boxCollider->Geometry;
        bool isGrounded = CollisionWorld.OverlapBox(localTransform.Position + OverlapDetectionOffset, new quaternion(0, 0, 0, 1), boxGeometry.Size / 2f, ref hits, boxCollider->GetCollisionFilter());

        playerMovementData.IsGrounded = isGrounded;
        foreach (var hit in hits)
        {
            Debug.DrawLine(localTransform.Position, hit.Position, Color.magenta, 2f);
        }
        hits.Dispose();
    }
}
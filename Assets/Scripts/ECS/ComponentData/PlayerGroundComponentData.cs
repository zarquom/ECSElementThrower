using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct PlayerGroundComponentData : IComponentData
{
    public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter GroundCollisionFilter;
}

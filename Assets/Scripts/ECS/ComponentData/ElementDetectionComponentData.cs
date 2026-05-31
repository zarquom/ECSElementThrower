using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct ElementDetectionComponentData : IComponentData
{
    public float3 OverlapDetectionOffset;
    public CollisionFilter DeadZoneCollisionFilter;
    public CollisionFilter EnemyCollisionFilter;
}

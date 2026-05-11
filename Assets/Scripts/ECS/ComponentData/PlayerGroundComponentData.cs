using Unity.Entities;
using Unity.Mathematics;

public struct PlayerGroundComponentData : IComponentData
{
    public float3 OverlapDetectionOffset;
}

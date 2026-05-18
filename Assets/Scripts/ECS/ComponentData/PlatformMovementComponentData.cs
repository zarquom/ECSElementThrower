using Unity.Entities;
using Unity.Mathematics;

public struct PlatformMovementComponentData : IComponentData
{
    public float3 InitialPosition;
    public float3 MovementVector;
    public float MovementSpeed;
    public bool IsReverseMovement;
}

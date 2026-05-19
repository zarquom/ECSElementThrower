using Unity.Entities;
using Unity.Jobs;

public struct PlayerMovementComponentData : IComponentData
{
    public float Direction;
    public bool IsJump;
    public bool IsGrounded;
    public Entity GroundHitEntity;
}

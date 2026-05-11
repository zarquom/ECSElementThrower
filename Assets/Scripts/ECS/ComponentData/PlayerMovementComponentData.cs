using Unity.Entities;

public struct PlayerMovementComponentData : IComponentData
{
    public float Direction;
    public bool IsJump;
    public bool IsGrounded;
}

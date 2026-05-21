using Unity.Entities;
using Unity.Physics;

public struct JumpingEnemyComponentData : IComponentData
{
    public float JumpForce;
    public double IdleTime;
    public double IdleFinishTime;
    public JumpingEnemyState State;
    public CollisionFilter GroundCollisionFilter;
}
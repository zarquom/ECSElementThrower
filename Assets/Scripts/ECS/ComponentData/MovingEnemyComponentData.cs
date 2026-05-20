using Unity.Entities;
using Unity.Physics;

public struct MovingEnemyComponentData : IComponentData
{
    public float MovementSpeed;
    public double IdleTime;
    public double IdleFinishTime;
    public MovingEnemyState State;
    public CollisionFilter GroundCollisionFilter;
}
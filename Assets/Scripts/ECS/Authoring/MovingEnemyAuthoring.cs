using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class MovingEnemyAuthoring : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private double _idleTime;
    [SerializeField] private PhysicsCategoryTags _groundBelongsTo;
    [SerializeField] private PhysicsCategoryTags _groundCollidesWith;
    public float MovementSpeed => _movementSpeed;
    public double idleTime => _idleTime;
    public CollisionFilter GroundCollisionFilter => new CollisionFilter
    {
        BelongsTo = _groundBelongsTo.Value,
        CollidesWith = _groundCollidesWith.Value 
    };

}
public class MovingEnemyBaker : Baker<MovingEnemyAuthoring>
{
    public override void Bake(MovingEnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new MovingEnemyComponentData
        {
            MovementSpeed = authoring.MovementSpeed,
            IdleTime = authoring.idleTime,
            GroundCollisionFilter = authoring.GroundCollisionFilter
        });
    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class JumpingEnemyAuthoring : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private double _idleTime;
    [SerializeField] private PhysicsCategoryTags _groundBelongsTo;
    [SerializeField] private PhysicsCategoryTags _groundCollidesWith;
    public float JumpForce => _jumpForce;
    public double idleTime => _idleTime;
    public CollisionFilter GroundCollisionFilter => new CollisionFilter
    {
        BelongsTo = _groundBelongsTo.Value,
        CollidesWith = _groundCollidesWith.Value
    };
}
public class JumpingEnemyBaker : Baker<JumpingEnemyAuthoring>
{
    public override void Bake(JumpingEnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new JumpingEnemyComponentData
        {
            JumpForce = authoring.JumpForce,
            IdleTime = authoring.idleTime,
            GroundCollisionFilter = authoring.GroundCollisionFilter
        });
    }
}
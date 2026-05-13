using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class PlayerDetectionAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _overlapDetectionOffset;
    [SerializeField] private PhysicsCategoryTags _deadZoneBelongsTo;
    [SerializeField] private PhysicsCategoryTags _deadZoneCollideWith;
    [SerializeField] private PhysicsCategoryTags _groundBelongsTo;
    [SerializeField] private PhysicsCategoryTags _groundCollideWith;
    [SerializeField] private PhysicsCategoryTags _endflagBelongsTo;
    [SerializeField] private PhysicsCategoryTags _endflagCollideWith;

    public float3 OverlapDetectionOffset => _overlapDetectionOffset;

    public CollisionFilter DeadZoneCollisionFilter => new CollisionFilter()
    {
        BelongsTo = _deadZoneBelongsTo.Value,
        CollidesWith = _deadZoneCollideWith.Value
    };

    public CollisionFilter GroundCollisionFilter => new CollisionFilter()
    {
        BelongsTo = _groundBelongsTo.Value,
        CollidesWith = _groundCollideWith.Value
    };
    public CollisionFilter EndFlagCollisionFilter => new CollisionFilter()
    {
        BelongsTo = _endflagBelongsTo.Value,
        CollidesWith = _endflagCollideWith.Value
    };
    public class PlayerDetectionBaker : Baker<PlayerDetectionAuthoring>
    {
        public override void Bake(PlayerDetectionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerDetectionComponentData
            {
                OverlapDetectionOffset = authoring.OverlapDetectionOffset,
                DeadZoneCollisionFilter = authoring.DeadZoneCollisionFilter,
                GroundCollisionFilter = authoring.GroundCollisionFilter,
                EndFlagCollisionFilter = authoring.EndFlagCollisionFilter
            });
        }
    }
}
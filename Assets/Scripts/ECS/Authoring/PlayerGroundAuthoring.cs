using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class PlayerGroundAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _overlapDetectionOffset;
    [SerializeField] private PhysicsCategoryTags _deadZoneBelongsTo;
    [SerializeField] private PhysicsCategoryTags _deadZoneCollideWith;
    [SerializeField] private PhysicsCategoryTags _groundBelongsTo;
    [SerializeField] private PhysicsCategoryTags _groundCollideWith;

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

    public class PlayerGroundBaker : Baker<PlayerGroundAuthoring>
    {
        public override void Bake(PlayerGroundAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerGroundComponentData
            {
                OverlapDetectionOffset = authoring.OverlapDetectionOffset,
                DeadZoneCollisionFilter = authoring.DeadZoneCollisionFilter,
                GroundCollisionFilter = authoring.GroundCollisionFilter
            });
        }
    }
}
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class ElementDetectionAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _overlapDetectionOffset;
    [SerializeField] private PhysicsCategoryTags _sharedBelongsTo;
    [SerializeField] private PhysicsCategoryTags _deadZoneCollideWith;
    [SerializeField] private PhysicsCategoryTags _enemyCollideWith;

    public float3 OverlapDetectionOffset => _overlapDetectionOffset;

    public CollisionFilter EnemyCollisionFilter => new CollisionFilter()
    {
        BelongsTo = _sharedBelongsTo.Value,
        CollidesWith = _enemyCollideWith.Value
    };
    public CollisionFilter DeadZoneCollisionFilter => new CollisionFilter()
    {
        BelongsTo = _sharedBelongsTo.Value,
        CollidesWith = _deadZoneCollideWith.Value
    };
    public class ElementDetectionBaker : Baker<ElementDetectionAuthoring>
    {
        public override void Bake(ElementDetectionAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ElementDetectionComponentData
            {
                OverlapDetectionOffset = authoring.OverlapDetectionOffset,
                DeadZoneCollisionFilter = authoring.DeadZoneCollisionFilter,
                EnemyCollisionFilter = authoring.EnemyCollisionFilter
            });
        }
    }
}
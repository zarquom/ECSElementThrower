using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerGroundAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _overlapDetectionOffset;

    public float3 OverlapDetectionOffset => _overlapDetectionOffset;

    public class PlayerGroundBaker : Baker<PlayerGroundAuthoring>
    {
        public override void Bake(PlayerGroundAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerGroundComponentData
            {
                OverlapDetectionOffset = authoring.OverlapDetectionOffset
            });
        }
    }
}
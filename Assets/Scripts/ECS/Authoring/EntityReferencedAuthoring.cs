using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
#if UNITY_EDITOR
public class EntityReferencedAuthoring : MonoBehaviour
{
    [SerializeField] private List<UnityEditor.SceneAsset> _levels;

    public List<UnityEditor.SceneAsset> Levels => _levels;
}

public class EntityReferencedBaker : Baker<EntityReferencedAuthoring>
{
    public override void Bake(EntityReferencedAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var buffer = AddBuffer<EntitySceneReferenceBufferElementData>(entity);
        foreach (var item in authoring.Levels)
        {
            buffer.Add(new EntitySceneReferenceBufferElementData
            {
                EntSceneReference = new EntitySceneReference(item)
            });
        }
    }
}
#endif
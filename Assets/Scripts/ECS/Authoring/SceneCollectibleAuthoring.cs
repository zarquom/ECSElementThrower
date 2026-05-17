using Unity.Entities;
using UnityEngine;

public class SceneCollectibleAuthoring : MonoBehaviour
{
    [SerializeField] private CollectibleType _type;

    public CollectibleType Type => _type;
}

public class SceneCollectibleBaker : Baker<SceneCollectibleAuthoring>
{
    public override void Bake(SceneCollectibleAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new SceneCollectibleComponent
        {
            Type = authoring.Type
        });
    }
}
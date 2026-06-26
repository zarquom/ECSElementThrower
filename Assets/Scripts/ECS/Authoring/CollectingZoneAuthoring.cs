using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CollectingZoneAuthoring : MonoBehaviour
{
    [SerializeField] private CollectibleType _collectibleType;
    public CollectibleType CollectibleType => _collectibleType;

}
public class CollectingZoneBaker : Baker<CollectingZoneAuthoring>
{
    public override void Bake(CollectingZoneAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new CollectingZoneComponentData
        {
            CollectibleType = authoring.CollectibleType
        });
    }
}
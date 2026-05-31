using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ElementAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _bulletDirection;
    [SerializeField] private float3 _bulletRotation;
    [SerializeField] private float _bulletSpeed;

    public float BulletSpeed => _bulletSpeed;
    public float3 BulletDirection => _bulletDirection;
    public float3 BulletRotation => _bulletRotation;

}
public class BulletBaker : Baker<ElementAuthoring>
{
    public override void Bake(ElementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new ElementComponentData
        {
            BulletDirection = authoring.BulletDirection,
            BulletRotation = authoring.BulletRotation,
            BulletSpeed = authoring.BulletSpeed
        });
    }
}
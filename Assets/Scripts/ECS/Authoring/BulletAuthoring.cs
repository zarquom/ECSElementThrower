using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    [SerializeField] private float3 _bulletDirection;
    [SerializeField] private float3 _bulletRotation;
    [SerializeField] private float _bulletSpeed;

    public float BulletSpeed => _bulletSpeed;
    public float3 BulletDirection => _bulletDirection;
    public float3 BulletRotation => _bulletRotation;

}
public class BulletBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new BulletComponentData
        {
            BulletDirection = authoring.BulletDirection,
            BulletRotation = authoring.BulletRotation,
            BulletSpeed = authoring.BulletSpeed
        });
    }
}
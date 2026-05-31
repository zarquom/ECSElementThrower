

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bulletPrefabs;
    [SerializeField] private int _bulletCount;
    public List<GameObject> BulletPrefabs => _bulletPrefabs;
    public int BulletCount => _bulletCount;
}

public class BulletSpawnerBaker : Baker<ElementSpawnerAuthoring>
{
    public override void Bake(ElementSpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        var bulletBufferElementData = AddBuffer<BulletBufferElementData>(entity);
        foreach (var bulletPrefab in authoring.BulletPrefabs)
        {
            bulletBufferElementData.Add(new BulletBufferElementData
            {
                BulletPrefab = GetEntity(bulletPrefab, TransformUsageFlags.Dynamic),
            });
        }
        AddComponent(entity, new ElementSpawnerComponentData
        {
            BulletCount = authoring.BulletCount
        });
    }
}

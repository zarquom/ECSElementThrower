

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bulletPrefabs;
    [SerializeField] private int _bulletCount;
    public List<GameObject> BulletPrefabs => _bulletPrefabs;
    public int BulletCount => _bulletCount;
}

public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
{
    public override void Bake(BulletSpawnerAuthoring authoring)
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
        AddComponent(entity, new BulletSpawnerComponentData
        {
            BulletCount = authoring.BulletCount
        });
    }
}

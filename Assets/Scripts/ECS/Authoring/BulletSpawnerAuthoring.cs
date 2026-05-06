

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bulletPrefabs;
    [SerializeField] private int _bulletCount;
    public uint InitialIndex => (uint) new System.Random().Next(1, int.MaxValue);
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
            Instance = new Unity.Mathematics.Random(authoring.InitialIndex),
            BulletCount = authoring.BulletCount,
        });
    }
}

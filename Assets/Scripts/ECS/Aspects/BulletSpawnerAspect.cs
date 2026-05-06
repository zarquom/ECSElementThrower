using System;
using Unity.Entities;
using Unity.Mathematics;

public readonly partial struct BulletSpawnerAspect : IAspect
{
    private readonly RefRW<BulletSpawnerComponentData> _bulletSpawnerComponentData;
    private readonly DynamicBuffer<BulletBufferElementData> _bulletBufferElementData;
    public int BulletCount => _bulletSpawnerComponentData.ValueRO.BulletCount;

    public float3 GetRandomPosition()
    {
        RandomizeInstance();
        float randomPositionY = _bulletSpawnerComponentData.ValueRW.Instance.NextFloat(-10f,10f);
        float randomPositionX = _bulletSpawnerComponentData.ValueRW.Instance.NextFloat(-10f,10f);
        return new float3(randomPositionX, randomPositionY, z: 0);
    }

    public Entity GetRandomPrefab()
    {
        RandomizeInstance();
        int randomIndex = _bulletSpawnerComponentData.ValueRW.Instance.NextInt(0, _bulletBufferElementData.Length);
        return _bulletBufferElementData[randomIndex].BulletPrefab;
    }
    private void RandomizeInstance()
    {
        uint randomSeed = _bulletSpawnerComponentData.ValueRW.Instance.NextUInt(UInt32.MaxValue);
        _bulletSpawnerComponentData.ValueRW.Instance.InitState(randomSeed);
    }
}

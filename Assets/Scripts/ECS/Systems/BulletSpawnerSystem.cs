using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletSpawnerSystem : ISystem
{
    private bool _isEnabled;
    private Unity.Mathematics.Random Instance;

    public void OnCreate(ref SystemState state)
    {
        // Initialization logic if needed
        state.RequireForUpdate<BulletSpawnerComponentData>();
        Instance = new Unity.Mathematics.Random((uint)new System.Random().Next(1, int.MaxValue));
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (_isEnabled)
        {
            return;

        }
        _isEnabled = true;
        
        Entity bulletSpawnerEntity = SystemAPI.GetSingletonEntity<BulletSpawnerComponentData>();
        var bulletBufferElementData = state.EntityManager.GetBuffer<BulletBufferElementData>(bulletSpawnerEntity);
        BulletSpawnerComponentData bulletSpawnerComponentData = SystemAPI.GetComponent<BulletSpawnerComponentData>(bulletSpawnerEntity);
        for (int i = 0; i < bulletSpawnerComponentData.BulletCount; i++)
        {
            float3 randomPos = GetRandomPosition(bulletSpawnerComponentData);
            Entity bulletEntity = state.EntityManager.Instantiate(GetRandomPrefab(bulletSpawnerComponentData, bulletBufferElementData));
            BulletComponentData bulletComponentData = SystemAPI.GetComponent<BulletComponentData>(bulletEntity);
            LocalTransform localTransform = LocalTransform.FromPositionRotationScale(randomPos, quaternion.Euler(bulletComponentData.BulletRotation, math.RotationOrder.XYZ), 0.2f);
            state.EntityManager.SetComponentData(bulletEntity, localTransform);
        }
    }

    public float3 GetRandomPosition(BulletSpawnerComponentData _bulletSpawnerComponentData)
    {
        RandomizeInstance(_bulletSpawnerComponentData);
        float randomPositionY = Instance.NextFloat(-10f, 10f);
        float randomPositionX = Instance.NextFloat(-10f, 10f);
        return new float3(randomPositionX, randomPositionY, z: -5);
    }

    public Entity GetRandomPrefab(BulletSpawnerComponentData _bulletSpawnerComponentData, DynamicBuffer<BulletBufferElementData> _bulletBufferElementData)
    {
        RandomizeInstance(_bulletSpawnerComponentData);
        int randomIndex = Instance.NextInt(0, _bulletBufferElementData.Length);
        return _bulletBufferElementData[randomIndex].BulletPrefab;
    }
    private void RandomizeInstance(BulletSpawnerComponentData _bulletSpawnerComponentData)
    {
        uint randomSeed = Instance.NextUInt(UInt32.MaxValue);
        Instance.InitState(randomSeed);
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Cleanup logic if needed
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletSpawnerSystem : ISystem
{
    private bool _isEnabled;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Initialization logic if needed
        state.RequireForUpdate<BulletSpawnerComponentData>();
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
        BulletSpawnerAspect bulletSpawnerAspect = SystemAPI.GetAspect<BulletSpawnerAspect>(bulletSpawnerEntity);
        for (int i = 0; i < bulletSpawnerAspect.BulletCount; i++)
        {
            float3 randomPos = bulletSpawnerAspect.GetRandomPosition();
            Entity bulletEntity = state.EntityManager.Instantiate(bulletSpawnerAspect.GetRandomPrefab());
            BulletAspect bulletAspect = state.EntityManager.GetAspect<BulletAspect>(bulletEntity);
            LocalTransform localTransform = LocalTransform.FromPositionRotationScale(randomPos, quaternion.Euler(bulletAspect.BulletRotation, math.RotationOrder.XYZ), 0.2f);
            state.EntityManager.SetComponentData(bulletEntity, localTransform);
        }
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Cleanup logic if needed
    }
}

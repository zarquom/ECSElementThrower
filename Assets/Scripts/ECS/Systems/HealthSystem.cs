using System;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(PlayerDamageSystem))]
public partial class HealthSystem : SystemBase
{
    public event Action<float> HealthUpdated;
    private EntityQuery _healthEntityQuery;
    private LevelSystem _levelSystem;

    protected override void OnCreate()
    {
        _healthEntityQuery = SystemAPI.QueryBuilder().WithAll<HealthComponentData>().Build();

        RequireForUpdate<PlayerComponentData>();
    }
    protected override void OnStartRunning()
    {
        _levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        _levelSystem.LevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        var playerComponentData = SystemAPI.GetSingleton<PlayerComponentData>();
        HealthUpdated?.Invoke(playerComponentData.Health);
    }

    protected override void OnUpdate()
    {
        NativeArray<Entity> healthEntities = _healthEntityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<HealthComponentData> healthComponents = _healthEntityQuery.ToComponentDataArray<HealthComponentData>(Allocator.Temp);

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        var playerComponentData = SystemAPI.GetComponent<PlayerComponentData>(playerEntity);

        for (int i = 0; i < healthComponents.Length; i++)
        {
            var healthEntity = healthEntities[i];
            var healthComponent = healthComponents[i];
            playerComponentData.Health += healthComponent.Value;
            EntityManager.DestroyEntity(healthEntity);
        }

        if(playerComponentData.Health <= 0)
        {
            playerComponentData.IsDead = true;
        }

        EntityManager.SetComponentData(playerEntity, playerComponentData);
        HealthUpdated?.Invoke(playerComponentData.Health);

        healthEntities.Dispose();
        healthComponents.Dispose();
    }
    protected override void OnStopRunning()
    {
        _levelSystem.LevelLoaded -= OnLevelLoaded;
    }
}

using System;
using Unity.Entities;
using Unity.VisualScripting;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class GameOverSystem : SystemBase
{
    public event Action<bool> GameOver;

    private LevelSystem _levelSystem;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponentData>();
    }

    protected override void OnStartRunning()
    {
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        _levelSystem.LastLevelCompleted += OnLastLevelCompleted;
    }

    private void OnLastLevelCompleted()
    {
        GameOver?.Invoke(true);
    }

    protected override void OnStopRunning()
    {
        _levelSystem.LastLevelCompleted -= OnLastLevelCompleted;
    }

    protected override void OnUpdate()
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        var playerComponentData = SystemAPI.GetComponent<PlayerComponentData>(playerEntity);

        if (!playerComponentData.IsDead)
        {
            return;
        }

        GameOver?.Invoke(false);
        //EntityManager.DestroyEntity(playerEntity);
    }
}

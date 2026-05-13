using System;
using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public event Action NextLevel;
    protected override void OnCreate()
    {
        RequireForUpdate<NextLevelComponentData>();
    }
    protected override void OnUpdate()
    {
        var nextLevelEntity = SystemAPI.GetSingletonEntity<NextLevelComponentData>();
        var nextLevelComponent = SystemAPI.GetComponent<NextLevelComponentData>(nextLevelEntity);

        if (nextLevelComponent.IsInvoked) return;

        NextLevel?.Invoke();
        nextLevelComponent.IsInvoked = true;
        EntityManager.SetComponentData<NextLevelComponentData>(nextLevelEntity, nextLevelComponent);
    }
}

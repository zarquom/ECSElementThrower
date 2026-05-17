using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct CollectibleSystem : ISystem, ISystemStartStop
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CollectibleComponentData>();
    }
    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        var collectibleCOmponetData = SystemAPI.GetSingleton<CollectibleComponentData>();
        ref var collectiblePool = ref collectibleCOmponetData.CollectiblePoolReference.Value;
        ref var collectibleDataArray = ref collectiblePool.CollectibleData;

        for (int i = 0; i < collectibleDataArray.Length; i++)
        {
            var collectibleContainer = collectibleDataArray[i];
            Debug.Log($"Collectible Points: {collectibleContainer.Points}");
        }
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

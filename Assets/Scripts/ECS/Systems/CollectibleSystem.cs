using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PlayerDetectionSystem))]
public partial struct CollectibleSystem : ISystem
{
    private EntityQuery collectedCollectibleEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CollectibleComponentData>();
        collectedCollectibleEntityQuery = SystemAPI.QueryBuilder().WithAll<CollectedCollectibleComponentData>().Build();
        state.RequireForUpdate(collectedCollectibleEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> collectedCollectibles = collectedCollectibleEntityQuery.ToEntityArray(Allocator.Temp);
        var collectibleCOmponetData = SystemAPI.GetSingleton<CollectibleComponentData>();
        ref var collectiblePool = ref collectibleCOmponetData.CollectiblePoolReference.Value;
        ref var collectibleDataArray = ref collectiblePool.CollectibleData;

        for (int i = 0; i < collectedCollectibles.Length; i++)
        {
            ExecuteCollectibleLogic(ref state, collectedCollectibles, i, ref collectibleDataArray);
        }
        collectedCollectibles.Dispose();
    }

    private void ExecuteCollectibleLogic(ref SystemState state, NativeArray<Entity> collectedCollectibles, int i, ref BlobArray<CollectibleContainer> collectibleDataArray)
    {
        var entity = collectedCollectibles[i];
        var sceneCollectibleComponentData = SystemAPI.GetComponent<SceneCollectibleComponent>(entity);
        for (int j = 0; j < collectibleDataArray.Length; j++)
        {
            var collectibleContainer = collectibleDataArray[j];
            if(collectibleContainer.Type == sceneCollectibleComponentData.Type)
            {
                //Debug.Log($"Points: " + collectibleContainer.Points);
                state.EntityManager.DestroyEntity(entity);
                var pointsEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(pointsEntity, new PointsComponentData
                {
                    Points = collectibleContainer.Points
                });
                break;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

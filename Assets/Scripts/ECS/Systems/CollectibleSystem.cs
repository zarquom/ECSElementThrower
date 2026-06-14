using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PlayerDetectionSystem))]
public partial struct CollectibleSystem : ISystem
{
    private EntityQuery obtainedPointsEntityQuery;
    private EntityQuery collectedCollectibleEntityQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CollectibleComponentData>();
        obtainedPointsEntityQuery = SystemAPI.QueryBuilder().WithAll<ObtainedPointsComponentData>().Build();
        collectedCollectibleEntityQuery = SystemAPI.QueryBuilder().WithAll<CollectedElementCollectibleComponentData>().Build();
        state.RequireForUpdate(obtainedPointsEntityQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*NativeArray<Entity> obtainedPointsCollectibles = obtainedPointsEntityQuery.ToEntityArray(Allocator.Temp);


        for (int i = 0; i < obtainedPointsCollectibles.Length; i++)
        {
            ExecutePointsLogic(ref state, obtainedPointsCollectibles, i, ref collectibleDataArray);
        }
        obtainedPointsCollectibles.Dispose();*/

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
                state.EntityManager.AddComponentData(pointsEntity, new ElementCollectibleComponentData
                {
                    Amount = (int)collectibleContainer.Points,
                    ElementType = collectibleContainer.Type
                });
                break;
            }
        }
    }

    private void ExecutePointsLogic(ref SystemState state, NativeArray<Entity> collectedCollectibles, int i, ref BlobArray<CollectibleContainer> collectibleDataArray)
    {
        var entity = collectedCollectibles[i];
        var sceneCollectibleComponentData = SystemAPI.GetComponent<SceneCollectibleComponent>(entity);
        for (int j = 0; j < collectibleDataArray.Length; j++)
        {
            var collectibleContainer = collectibleDataArray[j];
            if (collectibleContainer.Type == sceneCollectibleComponentData.Type)
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

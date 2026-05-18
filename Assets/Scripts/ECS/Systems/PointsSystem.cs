using System;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(CollectibleSystem))]
public partial class PointsSystem : SystemBase
{
    public event Action<float> PointsUpdated;

    private EntityQuery _pointsEntityQuery;
    private LevelSystem _levelSystem;
    private float _points;
    protected override void OnCreate()
    {
        _pointsEntityQuery = SystemAPI.QueryBuilder().WithAll<PointsComponentData>().Build();
        
    }
    protected override void OnStartRunning()
    {
        _levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        _levelSystem.LevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        _points = 0;
        PointsUpdated?.Invoke(_points);
    }

    protected override void OnStopRunning()
    {
        _levelSystem.LevelLoaded -= OnLevelLoaded;
    }

    protected override void OnUpdate()
    {
        NativeArray<Entity> pointsEntities = _pointsEntityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<PointsComponentData> pointsComponentArray = _pointsEntityQuery.ToComponentDataArray<PointsComponentData>(Allocator.Temp);
        for (int i = 0; i < pointsComponentArray.Length; i++)
        {
            var pointsEntity = pointsEntities[i];
            var pointsComponentData = pointsComponentArray[i];
            _points += pointsComponentData.Points;
            EntityManager.DestroyEntity(pointsEntity);
            PointsUpdated?.Invoke(_points);
        }
        pointsEntities.Dispose();
        pointsComponentArray.Dispose();
    }
}
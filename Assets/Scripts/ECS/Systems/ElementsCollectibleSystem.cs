using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateAfter(typeof(CollectibleSystem))]
public partial class ElementsCollectibleSystem : SystemBase
{
    public event Action<DynamicBuffer<CollectibleElement>> ElementsUpdated;

    private EntityQuery _elementsEntityQuery;
    private LevelSystem _levelSystem;
    protected override void OnCreate()
    {
        _elementsEntityQuery = SystemAPI.QueryBuilder().WithAll<ElementCollectibleComponentData>().Build();

    }
    protected override void OnStartRunning()
    {
        _levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        _levelSystem.LevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        ElementsUpdated?.Invoke(new DynamicBuffer<CollectibleElement>());
    }

    protected override void OnStopRunning()
    {
        _levelSystem.LevelLoaded -= OnLevelLoaded;
    }

    protected override void OnUpdate()
    {
        NativeArray<Entity> elementsEntities = _elementsEntityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<ElementCollectibleComponentData> elementsComponentArray = _elementsEntityQuery.ToComponentDataArray<ElementCollectibleComponentData>(Allocator.Temp);
        DynamicBuffer<CollectibleElement> buffer = new DynamicBuffer<CollectibleElement>();
        for (int i = 0; i < elementsComponentArray.Length; i++)
        {
            var elementsEntity = elementsEntities[i];
            var elementsComponentData = elementsComponentArray[i];
            buffer = OnElementPickedUp(elementsComponentData);
            EntityManager.DestroyEntity(elementsEntity);
        }
        ElementsUpdated?.Invoke(buffer);
        elementsEntities.Dispose();
        elementsComponentArray.Dispose();
    }
    private DynamicBuffer<CollectibleElement> OnElementPickedUp(ElementCollectibleComponentData elementCollectibleComponentData)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        DynamicBuffer<CollectibleElement> elementsBuffer = SystemAPI.GetBuffer<CollectibleElement>(player);
        for (int i = 0; i < elementsBuffer.Length; i++)
        {
            var element = elementsBuffer[i];
            if (element.Type == elementCollectibleComponentData.ElementType)
            {
                element.Amount += elementCollectibleComponentData.Amount;
                elementsBuffer[i] = element;
                break;
            }
        }
        return elementsBuffer;
    }
}
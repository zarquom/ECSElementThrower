using System;
using Unity.Entities;
using UnityEngine;

[RequireComponent (typeof(CollectiblesModel), typeof(CollectiblesView))]
public class CollectiblesController : MonoBehaviour
{
    private ElementsCollectibleSystem _collectiblesSystem;
    private CollectiblesModel _collectiblesModel;
    private CollectiblesView _collectiblesView;

    private void Awake()
    {
        _collectiblesModel = GetComponent<CollectiblesModel>();
        _collectiblesView = GetComponent<CollectiblesView>();
        _collectiblesSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ElementsCollectibleSystem>();
    }

    private void OnEnable()
    {
        _collectiblesSystem.ElementsUpdated += OnCollectiblesUpdated;
    }

    private void OnCollectiblesUpdated(DynamicBuffer<CollectibleElement> collectibles)
    {
        _collectiblesView.UpdateView(collectibles);
    }

    private void OnDisable()
    {
        _collectiblesSystem.ElementsUpdated -= OnCollectiblesUpdated;
    }
}

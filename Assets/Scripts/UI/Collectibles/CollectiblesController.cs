using Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequireComponent (typeof(CollectiblesModel), typeof(CollectiblesView))]
public class CollectiblesController : MonoBehaviour
{
    private ElementsCollectibleSystem _collectiblesSystem;
    private PlayerInputSystem _inputSystem;
    private CollectiblesModel _collectiblesModel;
    private CollectiblesView _collectiblesView;
    private List<CollectibleElement> lastCollectibles;
    private CollectibleType lastCollectible = CollectibleType.Fire;

    private void Awake()
    {
        _collectiblesModel = GetComponent<CollectiblesModel>();
        _collectiblesView = GetComponent<CollectiblesView>();
        _collectiblesSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<ElementsCollectibleSystem>();
        _inputSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerInputSystem>();
        lastCollectibles = new List<CollectibleElement>
        {
            new CollectibleElement {
                Type = CollectibleType.Fire,
                Amount = 0,
            },
            new CollectibleElement {
                Type = CollectibleType.Water,
                Amount = 0,
            },
            new CollectibleElement {
                Type = CollectibleType.Earth,
                Amount = 0,
            },
            new CollectibleElement {
                Type = CollectibleType.Wind,
                Amount = 0,
            }
        };
    }

    private void OnEnable()
    {
        _collectiblesSystem.ElementsUpdated += OnCollectiblesUpdated;
        _inputSystem.CollectibleTypeChanged += OnCollectibleSelectedUpdated;
    }

    private void OnCollectibleSelectedUpdated(CollectibleType type)
    {
        _collectiblesView.UpdateView(lastCollectibles, type);
        lastCollectible = type;
    }

    private void OnCollectiblesUpdated(DynamicBuffer<CollectibleElement> collectibles)
    {
        lastCollectibles.Clear();
        for (int i = 0; i < collectibles.Length; i++)
        {
            lastCollectibles.Add(collectibles[i]);
        }
        _collectiblesView.UpdateView(lastCollectibles, lastCollectible);
    }

    private void OnDisable()
    {
        _collectiblesSystem.ElementsUpdated -= OnCollectiblesUpdated;
        _inputSystem.CollectibleTypeChanged -= OnCollectibleSelectedUpdated;
    }
}

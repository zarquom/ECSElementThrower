using System;
using Unity.Entities;
using UnityEngine;

public class NextLevelWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _container;

    private LevelSystem _levelSystem;
    private void Awake()
    {
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        _container.SetActive(false);
    }

    private void OnEnable()
    {
        _levelSystem.NextLevel += OnNextLevelEvent;
    }

    private void OnDisable()
    {
        _levelSystem.NextLevel -= OnNextLevelEvent;
    }

    private void OnNextLevelEvent()
    {
        _container.SetActive(true);
    }
}

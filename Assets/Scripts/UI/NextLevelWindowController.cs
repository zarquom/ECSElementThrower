using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _container;
    [SerializeField] private Button _nextLevelButton;

    private LevelSystem _levelSystem;
    private void Awake()
    {
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        _container.SetActive(false);
    }

    private void OnClickNextlevel()
    {
        _levelSystem.LoadNextLevel();
        _container.SetActive(false);
    }

    private void OnEnable()
    {
        _levelSystem.NextLevel += OnNextLevelEvent;
        _nextLevelButton.onClick.AddListener(OnClickNextlevel);
    }

    private void OnDisable()
    {
        _levelSystem.NextLevel -= OnNextLevelEvent;
        _nextLevelButton.onClick.RemoveListener(OnClickNextlevel);
    }

    private void OnNextLevelEvent()
    {
        _container.SetActive(true);
    }
}

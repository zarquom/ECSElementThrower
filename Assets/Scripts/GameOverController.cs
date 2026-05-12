using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Localization;

public class GameOverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _text;
    [Header("Localize Texts")]
    [SerializeField] private LocalizedString _winGameLocalization;
    [SerializeField] private LocalizedString _loseGameLocalization;

    private GameOverSystem _gameOverSystem;

    private void Awake()
    {
        _gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
        _container.SetActive(false);
    }

    private void OnEnable()
    {
        _gameOverSystem.GameOver += OnGameOverEvent;
    }
    private void OnDisable()
    {
        _gameOverSystem.GameOver -= OnGameOverEvent;
    }
    private void OnGameOverEvent(bool isPlayerDead)
    {
        _container.SetActive(true);
        _text.text = isPlayerDead ? _winGameLocalization.GetLocalizedString() : _loseGameLocalization.GetLocalizedString();
    }
}

using System;
using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWindowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _container;
    [SerializeField] private Button _gameOverButton;
    [SerializeField] private TextMeshProUGUI _text;
    [Header("Localize Texts")]
    [SerializeField] private LocalizedString _winGameLocalization;
    [SerializeField] private LocalizedString _loseGameLocalization;

    private GameOverSystem _gameOverSystem;
    private LevelSystem _levelSystem;

    private void Awake()
    {
        _gameOverSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<GameOverSystem>();
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        _container.SetActive(false);
    }

    private void OnEnable()
    {
        _gameOverSystem.GameOver += OnGameOverEvent;
        _gameOverButton.onClick.AddListener(OnClickGameOver);
    }

    private void OnClickGameOver()
    {
        _levelSystem.UnloadPreviousLevel();
        _container.SetActive(false);
        StartCoroutine(LoadMenuScene());
    }

    IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(1f);
        _levelSystem.LoadScene(SceneType.Menu, LoadSceneMode.Additive);
    }

    private void OnDisable()
    {
        _gameOverSystem.GameOver -= OnGameOverEvent;
        _gameOverButton.onClick.RemoveListener(OnClickGameOver);
    }
    private void OnGameOverEvent(bool isPlayerDead)
    {
        _container.SetActive(true);
        _text.text = isPlayerDead ? _winGameLocalization.GetLocalizedString() : _loseGameLocalization.GetLocalizedString();
    }
}

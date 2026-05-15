using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;

    private LevelSystem _levelSystem;
    private void OnEnable()
    {
        _startGameButton.onClick.AddListener(OnClickStartGame);
        _quitGameButton.onClick.AddListener(OnClickQuit);
    }
    private void Awake()
    {
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
    }
    private void OnClickQuit()
    {
        Application.Quit();
    }

    private void OnClickStartGame()
    {
        _levelSystem.LoadScene(SceneType.Game, LoadSceneMode.Additive);
    }
    private void OnDisable()
    {
        _startGameButton.onClick.AddListener(OnClickStartGame);
        _quitGameButton.onClick.AddListener(OnClickQuit);
    }
}
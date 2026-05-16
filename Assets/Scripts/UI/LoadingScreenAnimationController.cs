using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScreenAnimationController : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private LevelSystem _levelSystem;

    private void OnEnable()
    {
        _levelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<LevelSystem>();
        _levelSystem.LevelLoaded += OnLevelLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetElementsAlpha(0f);
        Fade(1f);
    }

    private void OnDisable()
    {
        _levelSystem.LevelLoaded -= OnLevelLoaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Fade(float alpha)
    {
        _text.CrossFadeAlpha(alpha, LevelSystem.AnimationTimeInMillis / 1000f, false);
        _image.CrossFadeAlpha(alpha, LevelSystem.AnimationTimeInMillis / 1000f, false);
    }

    private void SetElementsAlpha(float alpha)
    {
        _text.canvasRenderer.SetAlpha(alpha);
        _image.canvasRenderer.SetAlpha(alpha);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.buildIndex == (int)SceneType.LoadingScreen) return;

        Fade(0f);
    }

    private void OnLevelLoaded()
    {
        Fade(0f);
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public const int AnimationTimeInMillis = 1000;

    public event Action LastLevelCompleted;
    public event Action LevelLoaded;
    public event Action NextLevel;

    private Entity _currentEntityScene;
    private SceneType _currentScene;
    private int _currentLevelIndex;

    public async void LoadScene(SceneType sceneType, LoadSceneMode sceneMode)
    {
        await LoadingScreen(true);
        if (_currentScene != SceneType.Main)
        {
            await SceneManager.UnloadSceneAsync((int)_currentScene).ToUniTask();
        }
        await SceneManager.LoadSceneAsync((int)sceneType, sceneMode).ToUniTask();
        _currentScene = sceneType;
        await LoadingScreen(false);
    }
    public async void LoadNextLevel(bool isLoadingScreen = true)
    {
        var entityReferencesDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        if (_currentLevelIndex > entityReferencesDynamicBuffer.Length - 1)
        {
            LastLevelCompleted?.Invoke();
            return;
        }
        if (isLoadingScreen)
            await LoadingScreen(true);
        await UnloadPreviousLevel();
        await LoadNextSubScene();
        await LoadingScreen(false);
    }

    private async UniTask LoadNextSubScene()
    {
        var entityReferencesDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        _currentEntityScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entityReferencesDynamicBuffer[_currentLevelIndex++].EntSceneReference);
        while (!SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, _currentEntityScene))
        {
            await UniTask.Yield();
        }
        LevelLoaded?.Invoke();
    }

    public async UniTask UnloadPreviousLevel()
    {
        if (Entity.Null.Equals(_currentEntityScene))
        {
            return;
        }
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, _currentEntityScene, SceneSystem.UnloadParameters.DestroyMetaEntities);
        while(SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, _currentEntityScene))
        {
            await UniTask.Yield();
        }
    }

    protected override void OnCreate()
    {
        RequireForUpdate<NextLevelComponentData>();
        SceneManager.sceneLoaded += SceneLoaded;
        LoadScene(SceneType.Menu, LoadSceneMode.Additive);
    }

    private void SceneLoaded(Scene sceneLoaded, LoadSceneMode loadSceneMode)
    {
        if (sceneLoaded.buildIndex != (int)SceneType.Game)
            return;

        _currentLevelIndex = 0;
        _currentEntityScene = Entity.Null;
        LoadNextLevel(false);
    }

    private async UniTask LoadingScreen(bool isEnabled)
    {
        if (isEnabled)
        {
            await SceneManager.LoadSceneAsync((int)SceneType.LoadingScreen, LoadSceneMode.Additive).ToUniTask();
        }

        await UniTask.Delay(AnimationTimeInMillis);

        if (!isEnabled)
        {
            await SceneManager.UnloadSceneAsync((int)SceneType.LoadingScreen).ToUniTask();
        }
    }
    protected override void OnUpdate()
    {
        var nextLevelEntity = SystemAPI.GetSingletonEntity<NextLevelComponentData>();
        var nextLevelComponent = SystemAPI.GetComponent<NextLevelComponentData>(nextLevelEntity);

        if (nextLevelComponent.IsInvoked) return;

        NextLevel?.Invoke();
        nextLevelComponent.IsInvoked = true;
        EntityManager.SetComponentData<NextLevelComponentData>(nextLevelEntity, nextLevelComponent);
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
}

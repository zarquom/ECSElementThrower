using System;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class LevelSystem : SystemBase
{
    public event Action LastLevelCompleted;
    public event Action LevelLoaded;
    public event Action NextLevel;

    private Entity _currentEntityScene;
    private SceneType _currentScene;
    private int _currentLevelIndex;

    public void LoadScene(SceneType sceneType, LoadSceneMode sceneMode)
    {
        if(_currentScene != SceneType.Main)
        {
            SceneManager.UnloadSceneAsync((int)_currentScene);
        }
        SceneManager.LoadSceneAsync((int)sceneType, sceneMode);
        _currentScene = sceneType;
    }
    public void LoadNextLevel()
    {
        var entityReferencesDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        if (_currentLevelIndex > entityReferencesDynamicBuffer.Length - 1)
        {
            LastLevelCompleted?.Invoke();
            return;
        }
        UnloadPreviousLevel();
        LoadNextSubScene();
    }

    private void LoadNextSubScene()
    {
        var entityReferencesDynamicBuffer = SystemAPI.GetSingletonBuffer<EntitySceneReferenceBufferElementData>();
        _currentEntityScene = SceneSystem.LoadSceneAsync(World.DefaultGameObjectInjectionWorld.Unmanaged, entityReferencesDynamicBuffer[_currentLevelIndex++].EntSceneReference);
        LevelLoaded?.Invoke();
    }

    public void UnloadPreviousLevel()
    {
        if (Entity.Null.Equals(_currentEntityScene))
        {
            return;
        }
        SceneSystem.UnloadScene(World.DefaultGameObjectInjectionWorld.Unmanaged, _currentEntityScene, SceneSystem.UnloadParameters.DestroyMetaEntities);
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
        LoadNextLevel();
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

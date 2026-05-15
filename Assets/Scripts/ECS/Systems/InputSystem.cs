using System;
using Unity.Entities;

public partial class InputSystem : SystemBase
{
    private InputControls _inputControls;
    private GameOverSystem _gameOverSystem;
    private LevelSystem _levelSystem;

    protected override void OnCreate()
    {
        _inputControls = new InputControls();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(_inputControls.PlayerMap);
    }

    protected override void OnStartRunning()
    {
        _gameOverSystem = EntityManager.World.GetExistingSystemManaged<GameOverSystem>();
        _levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        _gameOverSystem.GameOver += OnGameOverEvent;
        _levelSystem.NextLevel += OnNextLevelEvent;
        _levelSystem.LevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        _inputControls.Enable();
    }

    private void OnNextLevelEvent()
    {
        _inputControls.Disable();
    }

    private void OnGameOverEvent(bool isPlayerDead)
    {
        _inputControls.Disable();
    }
    protected override void OnStopRunning()
    {
        _gameOverSystem.GameOver -= OnGameOverEvent;
        _levelSystem.NextLevel -= OnNextLevelEvent;
        _levelSystem.LevelLoaded -= OnLevelLoaded;
    }
    protected override void OnUpdate()
    {

    }
}

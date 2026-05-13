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
        _inputControls.Enable();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(_inputControls.PlayerMap);
    }

    protected override void OnStartRunning()
    {
        _gameOverSystem = EntityManager.World.GetExistingSystemManaged<GameOverSystem>();
        _levelSystem = EntityManager.World.GetExistingSystemManaged<LevelSystem>();
        _gameOverSystem.GameOver += OnGameOverEvent;
        _levelSystem.NextLevel += OnNextLevelEvent;
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
    }
    protected override void OnUpdate()
    {

    }
}

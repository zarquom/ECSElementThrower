using Unity.Entities;

public partial class InputSystem : SystemBase
{
    private InputControls _inputControls;

    protected override void OnCreate()
    {
        _inputControls = new InputControls();
        _inputControls.Enable();
        var playerInputSystem = EntityManager.World.CreateSystemManaged<PlayerInputSystem>();
        playerInputSystem.SetPlayerMapActions(_inputControls.PlayerMap);
    }
    protected override void OnUpdate()
    {

    }
}

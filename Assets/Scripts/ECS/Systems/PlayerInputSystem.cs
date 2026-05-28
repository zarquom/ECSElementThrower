using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
public partial class PlayerInputSystem : SystemBase
{
    private InputControls.PlayerMapActions _playerMapActions;

    public void SetPlayerMapActions(InputControls.PlayerMapActions playerMapActions)
    {
        _playerMapActions = playerMapActions;
        _playerMapActions.Move.performed += OnMovePerformed;
        _playerMapActions.Move.canceled += OnMoveCancelled;
        _playerMapActions.Jump.performed += OnJumpPerformed;
        _playerMapActions.Throw.performed += OnThrowPerformed;
    }

    private void OnThrowPerformed(InputAction.CallbackContext callbackContext)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerComponentData>();
        PlayerComponentData playerData = SystemAPI.GetComponent<PlayerComponentData>(player);
        playerData.Throwing = true;
        SystemAPI.SetComponent(player, playerData);
    }

    private void OnMovePerformed (InputAction.CallbackContext callbackContext)
    {
        float movementDirection = callbackContext.ReadValue<float>();
        //Debug.Log($"Performed: {movementDirection}");
        Entity player = SystemAPI.GetSingletonEntity<PlayerMovementComponentData>();
        PlayerMovementComponentData playerMovement = SystemAPI.GetComponent<PlayerMovementComponentData>(player);
        playerMovement.Direction = movementDirection;
        playerMovement.LastDirection = movementDirection;
        SystemAPI.SetComponent(player, playerMovement);
    }
    private void OnJumpPerformed(InputAction.CallbackContext callbackContext)
    {
        Entity player = SystemAPI.GetSingletonEntity<PlayerMovementComponentData>();
        PlayerMovementComponentData playerMovement = SystemAPI.GetComponent<PlayerMovementComponentData>(player);
        playerMovement.IsJump = true;
        SystemAPI.SetComponent(player, playerMovement);
    }
    private void OnMoveCancelled(InputAction.CallbackContext callbackContext)
    {
        //Debug.Log("Cancelled");
        Entity player = SystemAPI.GetSingletonEntity<PlayerMovementComponentData>();
        PlayerMovementComponentData playerMovement = SystemAPI.GetComponent<PlayerMovementComponentData>(player);
        playerMovement.Direction = 0f;
        SystemAPI.SetComponent(player, playerMovement);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnDestroy()
    {
        _playerMapActions.Move.performed -= OnMovePerformed;
        _playerMapActions.Jump.performed -= OnJumpPerformed;
        _playerMapActions.Move.canceled -= OnMoveCancelled;
    }
}

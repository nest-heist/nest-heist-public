using System;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    private PlayerStateMachine _stateMachine;
    public event Action<InputAction.CallbackContext> OnMoveCanceledEvent;

    public PlayerInputHandler(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void AddInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled += OnMoveCanceled;
    }

    public void RemoveInputActionsCallbacks()
    {
        PlayerInput input = _stateMachine.Player.Input;
        input.PlayerActions.Move.canceled -= OnMoveCanceled;
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMoveCanceledEvent?.Invoke(context);
    }
}

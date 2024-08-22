using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarryState : PlayerBaseState
{
    public PlayerCarryState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        _inputHandler.OnMoveCanceledEvent += OnMovementCanceled;
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.CarryParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _inputHandler.OnMoveCanceledEvent -= OnMovementCanceled;
        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.CarryParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (_stateMachine.MovementDir != Vector2.zero && _stateMachine.Player.CarryingEgg)
        {
            _stateMachine.ChangeState(_stateMachine.CarryWalkState);
        }
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        if (_stateMachine.MovementDir == Vector2.zero) return;

        _stateMachine.ChangeState(_stateMachine.CarryIdleState);
    }
}

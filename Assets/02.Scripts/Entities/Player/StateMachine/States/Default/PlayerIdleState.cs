using UnityEngine;

public class PlayerIdleState : PlayerDefaultState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.IsIdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.IsIdleParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (_stateMachine.MovementDir != Vector2.zero)
        {
            _stateMachine.ChangeState(_stateMachine.WalkState);
        }
    }
}

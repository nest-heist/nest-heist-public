using UnityEngine;

public class PlayerDieState : PlayerBaseState
{
    public PlayerDieState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.SetTriggerAnimation(_stateMachine.Player.AnimeData.IsDieParameterHash);
        _stateMachine.Player.Rigid.velocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
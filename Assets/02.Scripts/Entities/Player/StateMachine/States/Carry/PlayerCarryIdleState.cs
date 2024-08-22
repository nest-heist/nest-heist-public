using UnityEngine;

public class PlayerCarryIdleState : PlayerCarryState
{
    public PlayerCarryIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.IsCarryIdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.IsCarryIdleParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }
}

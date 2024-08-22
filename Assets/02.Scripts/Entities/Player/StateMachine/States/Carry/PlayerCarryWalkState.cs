public class PlayerCarryWalkState : PlayerCarryState
{
    public PlayerCarryWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private float _carryWalkSpeedModifier = 0.75f;
    public override void Enter()
    {
        base.Enter();
        _stateMachine.MovementSpeedModifier = _carryWalkSpeedModifier;
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.IsCarryWalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.IsCarryWalkParameterHash);
    }
}

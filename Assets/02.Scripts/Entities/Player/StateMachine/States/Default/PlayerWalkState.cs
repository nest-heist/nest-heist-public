public class PlayerWalkState : PlayerDefaultState
{
    public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private float _walkSpeedModifier = 1f;
    public override void Enter()
    {
        base.Enter();
        _stateMachine.MovementSpeedModifier = _walkSpeedModifier;
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.IsWalkParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.IsWalkParameterHash);
    }
}

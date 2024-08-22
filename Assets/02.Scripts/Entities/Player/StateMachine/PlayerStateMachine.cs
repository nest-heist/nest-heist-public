using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; private set; }
    public Vector2 MovementDir { get; set; }
    public float MovementSpeedModifier { get; set; }
    public bool IsFacingRight { get; set; }
    public bool CanMove { get; set; }

    public PlayerDefaultState DefaultState { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }

    public PlayerCarryState CarryState { get; private set; }
    public PlayerCarryIdleState CarryIdleState { get; private set; }
    public PlayerCarryWalkState CarryWalkState { get; private set; }

    public PlayerHitState HitState { get; private set; }
    public PlayerDieState DieState { get; private set; }

    public PlayerStateMachine(Player player)
    {
        Player = player;
        CanMove = true;

        DefaultState = new PlayerDefaultState(this);
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);

        CarryState = new PlayerCarryState(this);
        CarryIdleState = new PlayerCarryIdleState(this);
        CarryWalkState = new PlayerCarryWalkState(this);

        HitState = new PlayerHitState(this);
        DieState = new PlayerDieState(this);
    }
}
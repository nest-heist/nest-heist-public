using UnityEngine;

public class PlayerMovementHandler
{
    protected PlayerStateMachine _stateMachine;
    private bool _isFacingRight = true;

    public PlayerMovementHandler(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Movement(Vector2 dir)
    {
        //if (_stateMachine.Player.Health.IsDie) return;
        Move(dir);
        UpdateSpriteDirection(dir);
    }

    protected virtual void Move(Vector2 dir)
    {
        float movementSpeed = GetMovementSpeed();
        if (!_stateMachine.CanMove) movementSpeed = 0;
        Vector2 externalForce = _stateMachine.Player.ForceReceiver.Movement;
        Vector2 totalForce = dir * movementSpeed + externalForce;
        _stateMachine.Player.Rigid.velocity = totalForce;
    }
    private float GetMovementSpeed()
    {
        float moveSpeed = _stateMachine.Player.StatHandler.CurrentStat.WalkSpeed * _stateMachine.MovementSpeedModifier;
        return moveSpeed;
    }

    protected virtual void UpdateSpriteDirection(Vector2 direction)
    {
        if (direction.x > 0.01 && !_isFacingRight)
        {
            FlipSprite(true);
        }
        else if (direction.x < -0.01 && _isFacingRight)
        {
            FlipSprite(false);
        }
    }

    protected virtual void FlipSprite(bool faceRight)
    {
        _isFacingRight = faceRight;
        _stateMachine.IsFacingRight = faceRight;
        Vector3 scale = _stateMachine.Player.MainSprite.localScale;
        scale.x = _isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        _stateMachine.Player.MainSprite.localScale = scale;
    }
}

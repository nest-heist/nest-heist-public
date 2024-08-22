using System.Collections;
using UnityEngine;

public class PlayerHitState : PlayerBaseState
{
    // 피격 상태 유지 시간, 추후 확장을 위해 public으로 설정(ex:무적시간 증가 등의 버프)
    public float HitDuration = 2.0f;

    public PlayerHitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _stateMachine.Player.CarryingEgg = false;
        _animeHandler.StartAnimation(_stateMachine.Player.AnimeData.IsHitParameterHash);
        _stateMachine.Player.StartCoroutine(HandleHitDuration());
        _stateMachine.Player.CarryingEgg = false;
        ChangeToIdleState();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    private IEnumerator HandleHitDuration()
    {
        yield return new WaitForSeconds(HitDuration);

        _animeHandler.StopAnimation(_stateMachine.Player.AnimeData.IsHitParameterHash);
    }

    private void ChangeToIdleState()
    {
        if (_stateMachine.Player.CarryingEgg)
        {
            _stateMachine.ChangeState(_stateMachine.CarryIdleState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}

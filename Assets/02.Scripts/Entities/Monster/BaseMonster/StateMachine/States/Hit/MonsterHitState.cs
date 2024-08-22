using System.Collections;
using UnityEngine;
public class MonsterHitState : MonsterBaseState
{
    // 피격 상태 유지 시간, 추후 확장을 위해 public으로 설정(ex:무적시간 증가 등의 버프)
    public WaitForSeconds HitDuration = new WaitForSeconds(1f);

    public MonsterHitState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.IsHitParameterHash);
        _stateMachine.Monster.StartCoroutine(HandleHitDuration());
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
        yield return HitDuration;
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.IsHitParameterHash);
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }

}

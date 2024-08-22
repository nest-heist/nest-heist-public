using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonsterPatrolState : MonsterIdleState
{
    public EnemyMonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.IsPatrolParameterHash);
        _stateMachine.Monster.StartCoroutine(_stateMachine.Monster.AIHandler.StartPatrol());
        _stateMachine.Monster.StartCoroutine(GoIdleState());
        //_stateMachine.Monster.AIHandler.StartPatrol();
    }

    public override void Exit()
    {
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.IsPatrolParameterHash);
        _stateMachine.Monster.StopCoroutine(_stateMachine.Monster.AIHandler.StartPatrol());
        _stateMachine.Monster.StopCoroutine(GoIdleState());
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Rotate();
    }
    public override void Update()
    {
        base.Update();
        if (_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.Monster.IsReturned = false;
            _stateMachine.ChangeState(_stateMachine.ChasingState);
            return;
        }
    }

    private IEnumerator GoIdleState()
    {
        yield return new WaitForSeconds(1f);
        _stateMachine.ChangeState(_stateMachine.IdleState);
    }
}

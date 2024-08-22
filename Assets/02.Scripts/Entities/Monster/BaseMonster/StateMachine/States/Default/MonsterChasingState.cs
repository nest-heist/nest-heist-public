using UnityEngine;

public class MonsterChasingState : MonsterDefaultState
{
    public MonsterChasingState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.IsChasingParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.IsChasingParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (_stateMachine.Monster.AIHandler.EnemyTargetTransform == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
        _stateMachine.ChangeState(_stateMachine.AttackState);
        return;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (_stateMachine.Monster.AIHandler.EnemyTargetTransform != null)
        {
            _stateMachine.Monster.AIHandler.Agent.SetDestination(_stateMachine.Monster.AIHandler.EnemyTargetTransform.position);
            if ((_stateMachine.Monster.AIHandler.EnemyTargetTransform.position - _stateMachine.Monster.transform.position).magnitude <= AttackRange)
            {
                _stateMachine.Monster.AIHandler.Agent.velocity = Vector2.zero;
            }
        }

        Rotate();
    }


}

using UnityEngine;

public class EnemyMonsterIdleState : MonsterIdleState
{
    public EnemyMonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
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
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget && _stateMachine.Monster.IsReturned)
        {
            _stateMachine.ChangeState(_stateMachine.PatrolState);
            return;
        }
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget && !_stateMachine.Monster.IsReturned)
        {
            _stateMachine.ChangeState(_stateMachine.ReturnState);
            return;
        }
    }
}

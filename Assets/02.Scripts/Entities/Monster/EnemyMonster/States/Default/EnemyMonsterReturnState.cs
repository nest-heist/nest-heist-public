using UnityEngine;

public class EnemyMonsterReturnState : MonsterIdleState
{
    public EnemyMonsterReturnState(MonsterStateMachine stateMachine) : base(stateMachine)
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
        _stateMachine.Monster.AIHandler.ReturnToOriginPosition();
        Rotate();
    }
    public override void Update()
    {
        base.Update();
        if (_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.ChangeState(_stateMachine.ChasingState);
            return;
        }
        if (_stateMachine.Monster.AIHandler.Agent.remainingDistance <= 1.5f)
        {
            _stateMachine.Monster.IsReturned = true;
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }
}

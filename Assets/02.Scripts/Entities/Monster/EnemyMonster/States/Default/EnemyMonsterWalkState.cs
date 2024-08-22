using UnityEngine;

public class EnemyMonsterWalkState : MonsterWalkState
{
    public EnemyMonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine)
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

    public override void Update()
    {
        base.Update();
        if (_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.ChangeState(_stateMachine.ChasingState);
            return;
        }
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
       
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

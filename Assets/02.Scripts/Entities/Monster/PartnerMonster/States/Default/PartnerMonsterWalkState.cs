public class PartnerMonsterWalkState : MonsterWalkState
{
    public PartnerMonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine)
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
        if (!_stateMachine.Monster.AIHandler.HasPlayerTarget)
        {
            return;
        }
        if (_stateMachine.Monster.AIHandler.HasEnemyTarget && _stateMachine.Monster.ActionModeHandler.GetActionMode() == Define.ActionMode.Combat)
        {
            _stateMachine.ChangeState(_stateMachine.ChasingState);
            return;
        }
        if (_stateMachine.Monster.AIHandler.HasPlayerTarget)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (_stateMachine.Monster.Player != null)
            _stateMachine.Monster.AIHandler.Agent.SetDestination(_stateMachine.Monster.Player.gameObject.transform.position);
    }
}

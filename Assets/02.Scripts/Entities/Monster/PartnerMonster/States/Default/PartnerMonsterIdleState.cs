public class PartnerMonsterIdleState : MonsterIdleState
{
    public PartnerMonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
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
    }
    public override void Update()
    {
        base.Update();
        if (!_stateMachine.Monster.AIHandler.HasPlayerTarget)
        {
            _stateMachine.ChangeState(_stateMachine.WalkState);
            return;
        }
        if (_stateMachine.Monster.AIHandler.HasEnemyTarget && _stateMachine.Monster.ActionModeHandler.GetActionMode() == Define.ActionMode.Combat)
        {
            _stateMachine.ChangeState(_stateMachine.ChasingState);
            return;
        }
    }
}

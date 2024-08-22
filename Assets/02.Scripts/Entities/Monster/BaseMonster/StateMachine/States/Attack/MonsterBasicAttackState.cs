using UnityEngine;
public class MonsterBasicAttackState : MonsterAttackState
{
    public MonsterBasicAttackState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.BasicAttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.BasicAttackParameterHash);

    }

    public override void Update()
    {
        base.Update();
        if (!IsBaseAttackRange())
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
        if (CheckAnimationDone(_stateMachine.Monster.AnimeData.AttackAnimName))
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        AttackRotate();
    }
}

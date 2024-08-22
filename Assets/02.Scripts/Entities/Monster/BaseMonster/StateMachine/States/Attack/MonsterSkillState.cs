using UnityEngine;
public class MonsterSkillState : MonsterAttackState
{
    public MonsterSkillState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.AttackParameterHash);
        UseSkill();
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.AttackParameterHash);
    }

    public override void Update()
    {
        base.Update();
        if (CheckAnimationDone("attack"))
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void UseSkill()
    {
        if (_stateMachine.Monster.AIHandler.EnemyTargetTransform != null)
        {
            _stateMachine.Monster.SkillHandler.Use(_stateMachine.Monster.AIHandler.EnemyTargetTransform.gameObject);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
    }
}

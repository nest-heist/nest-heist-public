using BehaviorDesigner.Runtime.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.AttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.AttackParameterHash);
    }
    public override void HandleInput()
    {
    }
    public override void Update()
    {
        base.Update();
        if (!_stateMachine.Monster.AIHandler.HasPlayerTarget)
        {
            _stateMachine.ChangeState(_stateMachine.WalkState);
            return;
        }
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
            return;
        }
        if (!_stateMachine.Monster.SkillHandler.GetIsCooldown() && !_stateMachine.Monster.IsPartner)
        {
            _stateMachine.ChangeState(_stateMachine.CastState);
            return;
        }
        if (IsBaseAttackRange())
        {
            _stateMachine.ChangeState(_stateMachine.BasicAttackState);
            return;
        }
        //if(스킬사용가능) 스킬사용
        //불가능하면 공격
    }

    public override void PhysicsUpdate()
    {
        AttackRotate();
    }

    //공격대상을 바라보고 회전
    protected void AttackRotate()
    {
        if (_stateMachine.Monster.AIHandler.EnemyTargetTransform == null) return;
        dir = (_stateMachine.Monster.AIHandler.EnemyTargetTransform.position - _stateMachine.Monster.transform.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _stateMachine.Monster.MainSprite.rotation = Quaternion.Euler(new Vector3(0, Mathf.Abs(angle) > 90 ? 0 : 180, 0));
    }
}

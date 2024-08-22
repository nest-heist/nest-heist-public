using BehaviorDesigner.Runtime.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class MonsterCastState : MonsterAttackState
{
    public MonsterCastState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.CastParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.CastParameterHash);
    }
    public override void Update()
    {
        base.Update();
        if (!_stateMachine.Monster.SkillHandler.GetIsCastTime())
        {
            _stateMachine.ChangeState(_stateMachine.SkillState);
            return;
        }
        //if(스킬사용가능) 스킬사용
        //불가능하면 공격
    }

    public override void PhysicsUpdate()
    {
        AttackRotate();
    }
}

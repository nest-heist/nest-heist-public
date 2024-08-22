using System.Collections;
using UnityEngine;
public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.SetTriggerAnimation(_stateMachine.Monster.AnimeData.IsDieParameterHash);
        _stateMachine.Monster.AIHandler.Agent.velocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

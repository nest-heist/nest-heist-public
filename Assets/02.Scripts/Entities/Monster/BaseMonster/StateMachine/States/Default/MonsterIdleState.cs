using UnityEngine;

public class MonsterIdleState : MonsterDefaultState
{
    public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.IsIdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.IsIdleParameterHash);

    }

    public override void Update()
    {
        base.Update();
        //Partner, Monster 따로 구현
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

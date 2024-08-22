using TMPro;
using UnityEngine;

public class MonsterWalkState : MonsterDefaultState
{
    public MonsterWalkState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.IsWalkParameterHash);
        _animeHandler.SetFloatAnimation(_stateMachine.Monster.AnimeData.WalkSpeedParameterHash, 2f);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.IsWalkParameterHash);

    }

    public override void Update()
    {
        base.Update();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Rotate();
    }


}

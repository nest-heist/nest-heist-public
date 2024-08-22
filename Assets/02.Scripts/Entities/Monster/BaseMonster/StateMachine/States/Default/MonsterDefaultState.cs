using UnityEngine;

public class MonsterDefaultState : MonsterBaseState
{
    public MonsterDefaultState(MonsterStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _animeHandler.StartAnimation(_stateMachine.Monster.AnimeData.DefaultParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        _animeHandler.StopAnimation(_stateMachine.Monster.AnimeData.DefaultParameterHash);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    protected void Rotate()
    {
        // 최우선 순위로 이동 방향을 바라봄
        Vector3 velocity = _stateMachine.Monster.AIHandler.Agent.velocity;
        if (velocity.sqrMagnitude >= 0.01f)
        {
            dir = velocity.normalized;
        }
        // 이동 속도가 거의 0이거나 이동 중이 아닌 경우 플레이어를 우선적으로 바라봄
        else if (_stateMachine.Monster.AIHandler.HasEnemyTarget)
        {
            dir = (_stateMachine.Monster.AIHandler.EnemyTargetTransform.position - _stateMachine.Monster.transform.position).normalized;
        }
        // 플레이어 추적중이 아닐 때 적을 바라봄
        else if (_stateMachine.Monster.AIHandler.HasPlayerTarget)
        {
            dir = (_stateMachine.Monster.AIHandler.TargetTransform.position - _stateMachine.Monster.transform.position).normalized;
        }
        // 타겟이 없으면 회전하지 않음
        else
        {
            return;
        }

        // 방향으로 각도 계산
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _stateMachine.Monster.MainSprite.rotation = Quaternion.Euler(new Vector3(0, Mathf.Abs(angle) > 90 ? 0 : 180, 0));
    }
}

using UnityEngine;

public abstract class MonsterBaseState : IState
{
    protected MonsterStateMachine _stateMachine;
    protected MonsterAnimeHandler _animeHandler;

    //회전할때 필요한 필드
    protected Vector2 dir;
    protected float angle;

    public float AttackRange = 0.5f;

    protected MonsterBaseState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _animeHandler = new MonsterAnimeHandler(_stateMachine);
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
    }

    protected bool IsBaseAttackRange()
    {
        //적이 존재하지 않다면
        if (!_stateMachine.Monster.AIHandler.HasEnemyTarget) return false;
        float distanceToTarget = (_stateMachine.Monster.AIHandler.EnemyTargetTransform.position - _stateMachine.Monster.transform.position).magnitude;
        if (distanceToTarget < AttackRange) return true;
        return false;
    }

    //애니메이션 시간 체크
    protected bool CheckAnimationDone(string animationName) 
    {
        if (_stateMachine.Monster.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            if (_stateMachine.Monster.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                return true;
            }
        }
        return false;
    }
}

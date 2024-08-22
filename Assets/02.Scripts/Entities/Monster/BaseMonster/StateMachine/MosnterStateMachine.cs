using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public BaseMonster Monster { get; private set; }


    public MonsterDefaultState DefaultState { get; private set; }
    public MonsterIdleState IdleState { get; private set; }
    public MonsterWalkState WalkState { get; private set; }
    public EnemyMonsterPatrolState PatrolState { get; private set; }
    public MonsterChasingState ChasingState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterBasicAttackState BasicAttackState { get; private set; }
    public MonsterCastState CastState { get; private set; }
    public MonsterSkillState SkillState { get; private set; }
    public MonsterHitState HitState { get; private set; }
    public MonsterDieState DieState { get; private set; }
    public EnemyMonsterReturnState ReturnState { get; private set; }
    public MonsterStateMachine(BaseMonster monster)
    {
        Monster = monster;

        if (Monster.GetType() == typeof(PartnerMonster))
        {
            IdleState = new PartnerMonsterIdleState(this);
            WalkState = new PartnerMonsterWalkState(this);
        }
        else if (Monster.GetType() == typeof(EnemyMonster))
        {
            IdleState = new EnemyMonsterIdleState(this);
            WalkState = new EnemyMonsterWalkState(this);
            ReturnState = new EnemyMonsterReturnState(this);
            PatrolState = new EnemyMonsterPatrolState(this);
        }

        DefaultState = new MonsterDefaultState(this);
        ChasingState = new MonsterChasingState(this);
        AttackState = new MonsterAttackState(this);
        CastState = new MonsterCastState(this);
        BasicAttackState = new MonsterBasicAttackState(this);
        SkillState = new MonsterSkillState(this);
        HitState = new MonsterHitState(this);
        DieState = new MonsterDieState(this);
    }
}
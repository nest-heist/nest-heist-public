using System;
using UnityEngine;

[Serializable]
public class MonsterAnimeData
{
    [SerializeField] string _defaultParameterName = "@Default";
    [SerializeField] string _isIdleParameterName = "IsIdle";
    [SerializeField] string _isWalkParameterName = "IsWalk";
    [SerializeField] string _isPatrolParameterName = "IsPatrol";
    [SerializeField] string _isChasingParameterName = "IsChasing";
    [SerializeField] string _attackParameterName = "@Attack";
    [SerializeField] string _castParameterName = "IsCasting";
    [SerializeField] string _basicAttackParameterName = "IsBasicAttack";
    [SerializeField] string _skillAttackParameterName = "IsSkillAttack";
    [SerializeField] string _ishitParameterName = "IsHit";
    [SerializeField] string _isdieParameterName = "IsDie";
    [SerializeField] string _walkSpeedParameterName = "WalkSpeed";


    public int DefaultParameterHash { get; private set; }
    public int IsIdleParameterHash { get; private set; }
    public int IsWalkParameterHash { get; private set; }
    public int IsPatrolParameterHash { get; private set; }
    public int IsChasingParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int CastParameterHash { get; private set; }
    public int BasicAttackParameterHash { get; private set; }
    public int SkillAttackParameterHash { get; private set; }
    public int IsHitParameterHash { get; private set; }
    public int IsDieParameterHash { get; private set; }
    public int WalkSpeedParameterHash { get; private set; }

    //애니메이션 체크를 위한 필드
    public string AttackAnimName = "attack";
    public string WalkAnimName = "Walk";
    public string ChaseAnimName = "Chasing";
    public string HitAnimName = "Hit";

    public MonsterAnimeData()
    {
        Initialize();
    }
    public void Initialize()
    {
        DefaultParameterHash = Animator.StringToHash(_defaultParameterName);
        IsIdleParameterHash = Animator.StringToHash(_isIdleParameterName);
        IsWalkParameterHash = Animator.StringToHash(_isWalkParameterName);
        IsPatrolParameterHash = Animator.StringToHash(_isPatrolParameterName);
        IsChasingParameterHash = Animator.StringToHash(_isChasingParameterName);

        AttackParameterHash = Animator.StringToHash(_attackParameterName);
        CastParameterHash = Animator.StringToHash(_castParameterName);
        BasicAttackParameterHash = Animator.StringToHash(_basicAttackParameterName);
        SkillAttackParameterHash = Animator.StringToHash(_skillAttackParameterName);

        IsHitParameterHash = Animator.StringToHash(_ishitParameterName);
        IsDieParameterHash = Animator.StringToHash(_isdieParameterName);

        WalkSpeedParameterHash = Animator.StringToHash(_walkSpeedParameterName);
    }
}
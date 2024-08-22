using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적과 파트너의 추적 AI 관련 로직의 베이스
/// </summary>
public class BaseMonsterAIHandler : MonoBehaviour
{
    [field: SerializeField] protected LayerMask _playerTarget { get; set; }
    //적 레이어
    [field: SerializeField] protected LayerMask _EnemyTarget { get; set; }
    protected BaseMonster _monster { get; set; }

    public NavMeshAgent Agent { get; set; }
    //플레이어 타겟
    public Transform TargetTransform { get; set; }
    public Transform EnemyTargetTransform { get; set; }
    protected Vector3 _originPosition { get; set; }
    protected float _detectRange = 5f;

    protected virtual void Start() { }
    public bool HasPlayerTarget
    {
        get { return TargetTransform != null; }
    }

    public bool HasEnemyTarget
    {
        get { return EnemyTargetTransform != null; }
    }

    public virtual void Init()
    {
        Agent = GetComponent<NavMeshAgent>();
        _monster = GetComponent<BaseMonster>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
    }

    private void Update()
    {
        UpdatePath();
    }

    //_updatePeriod마다 추적 갱신, TODO : privater => protected로 변경
    protected void UpdatePath()
    {
        UpdatePlayerTarget();
        UpdateEnemyTarget();
    }
    public void ReturnToOriginPosition()
    {
        Agent.SetDestination(_originPosition);
    }
    //Enemy, Partner에 따라서 다르게 행동
    //플레이어를 추적
    protected virtual void UpdatePlayerTarget()
    {
    }

    protected virtual void UpdateEnemyTarget()
    {
    }

    public virtual IEnumerator StartPatrol()
    {
        yield return null;
    }
}

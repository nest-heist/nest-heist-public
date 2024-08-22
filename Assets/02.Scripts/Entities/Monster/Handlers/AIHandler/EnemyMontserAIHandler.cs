using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMontserAIHandler : BaseMonsterAIHandler
{
    List<Vector3> PatrolPosList = new List<Vector3>();
    List<Vector3> SelectedPosList = new List<Vector3>();
    private int _patrolCount = 3;
    protected override void Start()
    {
        base.Start();
    }

    public override void Init()
    {
        base.Init();
        AddPatrolPos();
        _originPosition = transform.position;
        _EnemyTarget = LayerMask.GetMask(Define.PartnerLayer, Define.PlayerLayer);
    }
    protected override void UpdateEnemyTarget()
    {
        base.UpdateEnemyTarget();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, _EnemyTarget);
        if (colliders.Length > 0)
        {
            float closetDistance = Mathf.Infinity;
            Transform closetTarget = null;

            foreach (Collider2D collider in colliders)
            {
                float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToTarget < closetDistance)
                {
                    closetDistance = distanceToTarget;
                    closetTarget = collider.transform;
                }
                if (closetTarget != null)
                {
                    if ((1 << closetTarget.gameObject.layer) == LayerMask.GetMask(Define.PartnerLayer) ||
                        (1 << closetTarget.gameObject.layer) == LayerMask.GetMask(Define.PlayerLayer))
                    {
                        EnemyTargetTransform = closetTarget;
                        TargetTransform = closetTarget;
                    }
                }
                else
                {
                    EnemyTargetTransform = null;
                }
            }
        }
        else
        {
            EnemyTargetTransform = null;
        }
    }

    protected override void UpdatePlayerTarget()
    {
        base.UpdatePlayerTarget();
    }

    private void AddPatrolPos()
    {
        PatrolPosList.Add(new Vector3(2f, 0, 0));
        PatrolPosList.Add(new Vector3(2f, -2f, 0));
        PatrolPosList.Add(new Vector3(2f, 2f, 0));
        PatrolPosList.Add(new Vector3(0, 2f, 0));
        PatrolPosList.Add(new Vector3(0, 2f, 0));
        PatrolPosList.Add(new Vector3(-2f, 2f, 0));
        PatrolPosList.Add(new Vector3(-2f, 0, 0));
        PatrolPosList.Add(new Vector3(-2f, -2f, 0));
    }

    private void SelectPatrolPos()
    {
        List<Vector3> PatrolPosListCopy = new List<Vector3>(PatrolPosList);
        while (SelectedPosList.Count < _patrolCount)
        {
            int idx = Random.Range(0, PatrolPosListCopy.Count);
            SelectedPosList.Add(PatrolPosListCopy[idx]);
            PatrolPosListCopy.RemoveAt(idx);
        }
    }
    public override IEnumerator StartPatrol()
    {
        if (Agent.isActiveAndEnabled && Agent.isOnNavMesh)
        {
            SelectPatrolPos();
            int idx = Random.Range(0, SelectedPosList.Count);
            bool isTrue = Agent.SetDestination(_originPosition + SelectedPosList[idx]);

            if (!isTrue)
            {
                Debug.LogWarning("NavMeshAgent에서의 destination설정이 실패.");
            }
        }
        else
        {
            Debug.LogWarning("NavMeshAgent가 활성화가 안되어 있거나, 비활성화거나, 배치되지 않았다.");
        }

        yield return new WaitForSeconds(1f);
    }
}

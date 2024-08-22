using UnityEngine;


public class PartnerMonsterAIHandler : BaseMonsterAIHandler
{
    private float _enemyDetectRange = 5f;
    private bool _nearEnemy;

    //적 몬스터 타겟

    public override void Init()
    {
        base.Init();
        _detectRange = 2.5f;
        _playerTarget = LayerMask.GetMask(Define.PlayerLayer);
        _EnemyTarget = LayerMask.GetMask(Define.EnemyLayer);
        //지금 당장은 너무 느려서 *2를해준다.
        Agent.speed = GetComponent<BaseMonster>().StatHandler.CurrentStat.WalkSpeed * 5;
        _monster.ActionModeHandler.OnChangedMode += SetDetectRange;
    }

    public bool GetNearEnemy()
    {
        return _nearEnemy;
    }

    //적을 탐지 로직
    protected override void UpdateEnemyTarget()
    {
        base.UpdateEnemyTarget();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _enemyDetectRange, _EnemyTarget);
        if (colliders.Length > 0)
        {
            float closetDistance = Mathf.Infinity;
            Transform closetTarget = null;

            foreach (Collider2D collider in colliders)
            {
                float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);
                //타겟이 반경안에 있다면
                if (distanceToTarget < closetDistance)
                {
                    closetDistance = distanceToTarget;
                    closetTarget = collider.transform;
                }

                if (closetTarget != null)
                {
                    if ((1 << closetTarget.gameObject.layer) == LayerMask.GetMask("Enemy"))
                    {
                        EnemyTargetTransform = closetTarget;
                        _nearEnemy = true;
                    }
                }
                else
                {
                    //Agent.ResetPath();
                    EnemyTargetTransform = null;
                    _nearEnemy = false;
                }
            }
        }
        else
        {
            EnemyTargetTransform = null;
            _nearEnemy = false;
        }
    }

    //플레이어 탐지로직
    protected override void UpdatePlayerTarget()
    {
        base.UpdatePlayerTarget();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectRange, _playerTarget);
        if (colliders.Length > 0)
        {
            float closetDistance = Mathf.Infinity;
            Transform closetTarget = null;

            foreach (Collider2D collider in colliders)
            {
                float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);
                //타겟이 반경안에 있다면
                if (distanceToTarget < closetDistance)
                {
                    closetDistance = distanceToTarget;
                    closetTarget = collider.transform;
                }

                if (closetTarget != null)
                {
                    //타겟이 플레이어라면 
                    if ((1 << closetTarget.gameObject.layer) == LayerMask.GetMask("Player"))
                    {
                        TargetTransform = closetTarget;
                        //Agent.SetDestination(TargetTransform.position);             
                    }
                }
                else
                {
                    //Agent.ResetPath();
                    TargetTransform = null;
                }
            }
        }
        else
        {
            TargetTransform = null;
        }
    }

    //모드에 따라 플레이어 쫓는 범위 변경
    private void SetDetectRange()
    {
        if (_monster.ActionModeHandler.GetActionMode() == Define.ActionMode.NonCombat)
        {
            _detectRange = 2.5f;
        }
        else if (_monster.ActionModeHandler.GetActionMode() == Define.ActionMode.Combat)
        {
            _detectRange = 10f;
        }
    }
}

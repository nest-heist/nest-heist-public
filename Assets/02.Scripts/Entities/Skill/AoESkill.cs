using System.Collections;
using UnityEngine;

#nullable enable

/// <summary>
/// 원형으로 한번에 여러 대상 데미지 한 번 주기
/// </summary>
public class AoESkill : AttackSkill, IAreaSkill
{
    [field: SerializeField] public GameObject AOEObject { get; set; }
    [field: SerializeField] public float Radius { get; set; }
    [field: SerializeField] public float Duration { get; set; }

    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        ApplyAreaEffect(_targetPos);
    }

    public void ApplyAreaEffect(Vector2 targetPos)
    {
        AOEObject.transform.position = targetPos;
        AOEObject.SetActive(true);

        ApplyDamage(targetPos);
        StartCoroutine(DestroyAOEObjectAfterDelay(Duration));
    }

    public void ApplyDamage(Vector2 targetPos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(targetPos, Radius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable targetHealth = hitCollider.GetComponent<IDamageable>();
            //본인은 맞지 않게 설정
            if ((_targetLayer & (1 << hitCollider.gameObject.layer)) != 0)
            {
                continue;
            }
            if (targetHealth != null && !targetHealth.IsPlayer())
            {
                // null의 경우, 보스의 경우 예외처리
                string? monsterId = targetHealth.MonsterId;

                float attributeDamage = 1f;

                if (monsterId != null)
                {
                    attributeDamage = GameManager.Instance.MonsterAttributeSystem.SkillAttributeTypeMulitiplier(
                                      DataManager.Instance.ReadOnlyDataSystem.MonsterSkillInfo[_info.Id],
                                      DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[targetHealth.MonsterId]);
                }

                targetHealth.TakeDamage((int)(_monster.StatHandler.CurrentStat.Attack * _info.DamageMulti * attributeDamage));
            }

            if (targetHealth != null && targetHealth.IsPlayer())
            {
                targetHealth.TakeDamage((int)(_monster.StatHandler.CurrentStat.Attack * _info.DamageMulti));
            }
        }
    }

    protected override void SetCastArea(Vector2 targetPos)
    {
        CastAreaObject.transform.position = targetPos;
        CastAreaObject.transform.localScale = new Vector2(Radius, Radius);
    }

    private IEnumerator DestroyAOEObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AOEObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (_targetPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetPos, Radius);
        }
    }
}
using System.Collections;
using UnityEngine;

#nullable enable

/// <summary>
/// 원격 - 투사체 던지고 원형으로 범위 공격 스킬
/// </summary>
public class ProjectileAOESkill : AttackSkill, IProjectileSkill, IAreaSkill
{
    [field: SerializeField] public GameObject ProjectileObject { get; set; }
    [field: SerializeField] public GameObject AOEObject { get; set; }
    [field: SerializeField] public float ProjectileSpeed { get; set; }
    [field: SerializeField] public float Radius { get; set; }

    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        ApplyProjectileEffect(_targetPos);
    }

    public void ApplyProjectileEffect(Vector2 targetPos)
    {
        // 투사체 생성 및 발사
        ProjectileObject.transform.position = transform.position;

        ProjectileObject.GetComponent<Projectile>().Initialize(targetPos, ProjectileSpeed, ApplyAreaEffect);
    }

    public void ApplyAreaEffect(Vector2 targetPos)
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
        CastAreaObject.SetActive(false);
        AOEObject.transform.position = targetPos;
        AOEObject.SetActive(true);

        StartCoroutine(DestroyAOEObjectAfterDelay(1.0f));
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
        Gizmos.color = Color.red; // 기즈모 색상 설정
        Gizmos.DrawWireSphere(_targetPos, Radius); // 현재 위치에 원형 기즈모 그리기
    }
}

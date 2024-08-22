using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#nullable enable

/// <summary>
/// 원격 - 투사체로 타겟 공격 스킬
/// </summary>
public class ProjectileTargetSkill : AttackSkill, IProjectileSkill, ITargetSkill
{
    [field: SerializeField] public GameObject ProjectileObject { get; set; }
    [field: SerializeField] public GameObject AOEObject { get; set; }
    [field: SerializeField] public float ProjectileSpeed { get; set; }
    [field: SerializeField] public float Radius { get; set; }

    [field: SerializeField] public List<GameObject> ProjectileObjectList { get; set; }
    [field: SerializeField] public List<GameObject> AOEObjectList { get; set; }

    [field: SerializeField] public float MinX { get; set; }
    [field: SerializeField] public float MaxX { get; set; }
    [field: SerializeField] public float MaxY { get; set; }
    [field: SerializeField] public int DelaySecond { get; set; }

    private int _currentProjectileIndex = 0;

    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        ApplyProjectileEffect(_targetPos);
    }

    public async void ApplyProjectileEffect(Vector2 targetPos)
    {
        foreach (GameObject projectile in ProjectileObjectList)
        {
            float randomX = Random.Range(-MinX, MaxX);
            float randomY = Random.Range(0, MaxY);

            Vector2 offset = new Vector2(randomX, randomY);

            projectile.transform.position = targetPos;
            projectile.GetComponent<Projectile>().Initialize(targetPos + offset, ProjectileSpeed, ApplyTargetEffect);

            await Task.Delay(DelaySecond);
        }
    }

    public void ApplyTargetEffect(Vector2 targetPos)
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

        AOEObjectList[_currentProjectileIndex].transform.position = targetPos;
        AOEObjectList[_currentProjectileIndex].SetActive(true);

        StartCoroutine(DestroyAOEObjectAfterDelay(0.5f, _currentProjectileIndex));

        _currentProjectileIndex++;
        _currentProjectileIndex %= AOEObjectList.Count - 1;
    }

    private IEnumerator DestroyAOEObjectAfterDelay(float delay, int idx)
    {
        yield return new WaitForSeconds(delay);
        AOEObjectList[idx].SetActive(false);
    }

    protected override void SetCastArea(Vector2 targetPos)
    {
        CastAreaObject.transform.position = targetPos;
        CastAreaObject.transform.localScale = new Vector2(Radius, Radius);
    }
}
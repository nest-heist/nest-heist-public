using System.Collections;
using UnityEngine;

#nullable enable

/// <summary>
/// 근접 - 박스 형태로 범위 공격 스킬
/// </summary>
public class BoxAoEMeleeSkill : AttackSkill, IBoxAreaSkill
{
    [field: SerializeField] public GameObject AOEObject { get; set; }
    [field: SerializeField] public Vector2 BoxDimensions { get; set; }

    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);

        CastAreaObject.SetActive(false);
        ApplyBoxAreaEffect(_targetPos);
    }

    public void ApplyBoxAreaEffect(Vector2 targetPos)
    {
        AOEObject.transform.position = CastAreaObject.transform.position;
        AOEObject.transform.rotation = CastAreaObject.transform.rotation;
        AOEObject.SetActive(true);

        ApplyBoxDamage(targetPos);
        StartCoroutine(DestroyAOEObjectAfterDelay(1f)); // 일정 시간 후에 AOEObject를 비활성화
    }

    private void ApplyBoxDamage(Vector2 targetPos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(targetPos, BoxDimensions, AOEObject.transform.eulerAngles.z);
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
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        transform.right = direction;
        CastAreaObject.transform.rotation = transform.rotation;

        targetPos = transform.position + (transform.right * (BoxDimensions.x / 2));
        _targetPos = targetPos;
        CastAreaObject.transform.position = targetPos;
        CastAreaObject.transform.localScale = BoxDimensions;
    }

    private IEnumerator DestroyAOEObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AOEObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (AOEObject != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(_targetPos, CastAreaObject.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, BoxDimensions);
        }
    }
}

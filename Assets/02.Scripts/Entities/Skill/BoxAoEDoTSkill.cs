using System.Collections;
using UnityEngine;

#nullable enable

/// <summary>
/// 원격 - 박스 형태로 도트공격 스킬
/// </summary>
public class BoxAoEDoTSkill : AttackSkill, IBoxAreaSkill, IDoTSkill
{
    [field: SerializeField] public GameObject AOEObject { get; set; }
    [field: SerializeField] public float TickInterval { get; set; }
    [field: SerializeField] public float Duration { get; set; }
    [field: SerializeField] public Vector2 BoxDimensions { get; set; }

    private float _elapsedTime;
    private Vector2 _offsetPos;

    public override void Init(MonsterSkillInfoData info, LayerMask targetLayer, BaseMonster monster)
    {
        base.Init(info, targetLayer, monster);

        _offsetPos = AOEObject.transform.localPosition;
    }

    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        ApplyBoxAreaEffect(_targetPos);
    }

    public void ApplyBoxAreaEffect(Vector2 targetPos)
    {
        AOEObject.transform.position = targetPos + _offsetPos;
        AOEObject.SetActive(true);

        StartCoroutine(ApplyDoTDamageCoroutine(targetPos));
        StartCoroutine(DestroyAOEObjectAfterDelay(Duration));
    }

    private IEnumerator ApplyDoTDamageCoroutine(Vector2 targetPos)
    {
        _elapsedTime = 0f;

        CastAreaObject.SetActive(false);
        while (_elapsedTime < Duration)
        {
            ApplyDoTDamage(targetPos);
            _elapsedTime += TickInterval;
            yield return new WaitForSeconds(TickInterval);
        }
    }

    public void ApplyDoTDamage(Vector2 targetPos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(targetPos, BoxDimensions, 0);

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
        CastAreaObject.transform.localScale = BoxDimensions;
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
            Gizmos.matrix = Matrix4x4.TRS(_targetPos, CastAreaObject.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, BoxDimensions);
        }
    }
}

using System;
using System.ComponentModel;
using UnityEngine;

#nullable enable

public interface IAttack
{
    public void OnDamaging();
    //현재 타겟을 가져온다.
    public GameObject GetTarget();
}
/// <summary>
/// 적과 파트너의 공격 핸들러
/// 현재 AI가 바라보고있는 타겟정보를 가져와서 공격
/// </summary>
public abstract class AttackHandler : MonoBehaviour, IAttack
{
    public BaseMonster Monster;
    public int Damage;
    public float Critical;
    public int CriticalDamage;
    //최종 데미지
    public float FinalDamage;


    public void Init()
    {
        Monster = GetComponent<BaseMonster>();
        InitStat();
        if(Monster.Attack != null)
        {
            Monster.Attack.OnDamagingEvent += OnDamaging;
        }
    }

    public abstract GameObject GetTarget();

    //스텟정보를 가져온다.
    private void InitStat()
    {
        Damage = Monster.StatHandler.CurrentStat.Attack;
        Critical = Monster.StatHandler.CurrentStat.Critical;
        CriticalDamage = Monster.StatHandler.CurrentStat.CriticalDamage;
        FinalDamage = Damage;
    }
    //이펙트 정보도 가져온다.
    public virtual void OnDamaging()
    {
        GameObject target = GetTarget();
        if (target == null) return;
        CalculateCritical();

        if (target.TryGetComponent<IDamageable>(out IDamageable component))
        {
            if (component.IsDead()) return;
            if (!component.IsPlayer())
            {
                string? monsterId = component.MonsterId;

                float attributeDamage = 1f;

                if(monsterId != null)
                {
                    FinalDamage *= GameManager.Instance.MonsterAttributeSystem.CalculateDamageMultiplier(
                               DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[Monster.HealthSystem.MonsterId],
                               DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[component.MonsterId]) * attributeDamage;
                }
            }

            component.TakeDamage((int)FinalDamage);
            Monster.MontserEffectHandler.ActiveEffect("BaseAttack", Monster.MontserEffectHandler.BaseAttackTrasform);
            AudioManager.Instance.PlaySFX("Basic_Attack");
        }
        FinalDamage = Damage;
    }

    public void CalculateCritical()
    {
        // 난수 생성기 인스턴스 생성 (간단한 시드 사용)
        System.Random rng = new System.Random();

        // 0부터 99까지의 난수 생성
        int randomValue = rng.Next(100);
        if (Critical >= randomValue)
        {
            // CriticalDamage 값을 추가하여 최종 데미지를 계산
            FinalDamage = Damage + CriticalDamage;
        }
    }
}

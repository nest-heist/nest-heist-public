using System;
using UnityEngine;
/// <summary>
/// 몬스터 개체값 스텟
/// 각 개체값을 몬스터마다 스텟을 어떻게 나눠 가질지
/// </summary>
public class MonsterStatData : IReturnStat
{
    // 몬스터 아이디
    public string MonsterId;

    // 공격력
    public int Attack;

    // 최대체력
    public int MaxHP;

    // 방어력
    public int Defence;

    // 크리티컬 확률
    public float Critical;

    // 크리티컬 피해
    public int CriticalDamage;

    // 이동 속도
    public float WalkSpeed;


    public MonsterStat ReturnStat()
    {
        MonsterStat newBase = ScriptableObject.CreateInstance<MonsterStat>();
        newBase.Attack = Attack;
        newBase.MaxHP = MaxHP;
        newBase.Defence = Defence;
        newBase.Critical = Critical;
        newBase.CriticalDamage = CriticalDamage;
        newBase.WalkSpeed = WalkSpeed;

        return newBase;
    }
}

public interface IReturnStat
{
    public MonsterStat ReturnStat();
}
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BOSS", menuName = "BossStat")]
public class MonsterStat : ScriptableObject, IStat<MonsterStat>
{
    public int Attack;
    public int MaxHP;
    public int Defence;
    public float Critical;
    public int CriticalDamage;
    public float WalkSpeed;
    public StatType Type { get; set; }

    public MonsterStat DeepCopy()
    {
        return Instantiate(this);
    }

    public void Add(MonsterStat other)
    {
        Attack += other.Attack;
        MaxHP += other.MaxHP;
        Defence += other.Defence;
        Critical += other.Critical;
        CriticalDamage += other.CriticalDamage;
        WalkSpeed += other.WalkSpeed;
    }

    public void Multiply(MonsterStat other)
    {
        Attack *= other.Attack;
        MaxHP *= other.MaxHP;
        Defence *= other.Defence;
        Critical *= other.Critical;
        CriticalDamage *= other.CriticalDamage;
        WalkSpeed *= other.WalkSpeed;
    }

    public MonsterStat StatCalculator(Func<float, int, float> calculator, int num)
    {
        MonsterStat newStat = CreateInstance<MonsterStat>();
        newStat.Attack = (int)calculator(this.Attack, num);
        newStat.MaxHP = (int)calculator(this.MaxHP, num);
        newStat.Defence = (int)calculator(this.Defence, num);
        newStat.Critical = calculator(this.Critical, num);
        newStat.CriticalDamage = (int)calculator(this.CriticalDamage, num);
        newStat.WalkSpeed = calculator(this.WalkSpeed, num);
        return newStat;
    }

}
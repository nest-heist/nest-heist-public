using System;
using UnityEngine;


[Serializable]
public class PlayerStat : ScriptableObject, IStat<PlayerStat>
{
    // 플레이어 최대 체력
    public int MaxHP;
    // 플레이어 회피율
    public float Avoid;
    // 플레이어 이동 속도
    public float WalkSpeed;

    public float HPRegen = 1;

    public StatType Type { get; set; }


    public void Add(PlayerStat other)
    {
        MaxHP += other.MaxHP;
        Avoid += other.Avoid;
        WalkSpeed += other.WalkSpeed;
    }

    public PlayerStat DeepCopy()
    {
        return Instantiate(this);
    }

    public void Multiply(PlayerStat other)
    {
        MaxHP *= other.MaxHP;
        Avoid *= other.Avoid;
        WalkSpeed *= other.WalkSpeed;
    }

    public PlayerStat StatCalculator(Func<float, int, float> calculator, int num)
    {
        PlayerStat newStat = new PlayerStat();
        newStat.MaxHP = (int)calculator(this.MaxHP, num);
        newStat.Avoid = calculator(this.Avoid, num);
        newStat.WalkSpeed = calculator(this.WalkSpeed, num);
        return newStat;
    }
}




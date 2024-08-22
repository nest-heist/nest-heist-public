using System;
using UnityEngine;

public static class StatCalculator
{
    public static MonsterStat ConvertToMonsterStat(IReturnStat baseStat)
    {
        MonsterStat stat = baseStat.ReturnStat();

        return stat;
    }

    public static float CalculateIVStat(float value, int iv)
    {
        double logMultiplier = Math.Log(iv + 1);
        return (float)(value * logMultiplier);
    }

    public static float CalculateLevelStat(float value, int iv)
    {
        double logMultiplier = Math.Log(iv + 1);

        return (float)(value * logMultiplier);
    }

    public static float CalculateRankStat(float value, int iv)
    {
        double logMultiplier = Math.Log(iv + 1);
        return (float)(value * logMultiplier);
    }

    public static float PlayerCalculateLevelStat(float value, int iv)
    {
        double logMultiplier = Math.Log(iv + 1);
        return (float)(value * logMultiplier);
    }
}
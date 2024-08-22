using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : Singleton<CooldownManager>
{
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();

    /// <summary>
    /// 쿨다운 시작
    /// </summary>
    /// <param name="tag">쿨다운 항목 태그</param>
    /// <param name="cooldownTime">쿨다운 시간</param>
    public void StartCooldown(string tag, float cooldownTime)
    {
        float endTime = Time.time + cooldownTime;
        if (cooldowns.ContainsKey(tag))
        {
            cooldowns[tag] = endTime;
        }
        else
        {
            cooldowns.Add(tag, endTime);
        }
    }

    /// <summary>
    /// 쿨다운 중인지 확인
    /// </summary>
    /// <param name="tag">쿨다운 항목 태그</param>
    /// <returns>쿨다운 중이면 true, 아니면 false</returns>
    public bool IsCooldownActive(string tag)
    {
        if (!cooldowns.ContainsKey(tag))
        {
            return false;
        }

        return Time.time < cooldowns[tag];
    }

    /// <summary>
    /// 쿨다운 남은 시간 확인
    /// </summary>
    /// <param name="tag">쿨다운 항목 태그</param>
    /// <returns>남은 시간</returns>
    public float GetRemainingCooldown(string tag)
    {
        if (!cooldowns.ContainsKey(tag))
        {
            return 0f;
        }

        return Mathf.Max(0, cooldowns[tag] - Time.time);
    }
}

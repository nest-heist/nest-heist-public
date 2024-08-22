using System;

public static class SkillEvents
{
    public static event Action<float> OnSkillCooldownUpdate;

    public static void RaiseSkillCooldownUpdate(float remainingCooldown)
    {
        OnSkillCooldownUpdate?.Invoke(remainingCooldown);
    }
}

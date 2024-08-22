using System;
/// <summary>
/// 피로도 관련 데이터
/// </summary>
[Serializable]
public class ServerUserFatigueData
{
    public int MaxFatigue;
    public int CurrentFatigue;
    public string LastFatigueRecovery;
    public int LastFatigueRecoverySeconds;
}
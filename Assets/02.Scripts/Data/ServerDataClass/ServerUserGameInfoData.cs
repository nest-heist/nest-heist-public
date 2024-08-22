using System;

/// <summary>
/// 유저 데이터
/// 서버에서 받아오는 방식
/// </summary>
[Serializable]
public class ServerUserGameInfoData
{
    public string UserId;
    public string Username;
    public string PlayerId;
    public int Level;
    public int Exp;
    public int Gold;
    public string ProfilePicture;
    public string Status;
    public int MaxFatigue;
    public int CurrentFatigue;
    public string LastFatigueRecovery;
    public string PartnerMonsterId;
    public string[] PartnerMonsterIds;

    /// <summary>
    /// 처음 게임 시작할 때 초기화 해주는 값
    /// </summary>
    public ServerUserGameInfoData()
    {
        UserId = "PID00001";
        Username = "noName";
        Level = -1;
        Exp = 0;
        Gold = 0;
        ProfilePicture = "PID00001";
        Status = "noStatus";
        MaxFatigue = 0;
        CurrentFatigue = 0;
        LastFatigueRecovery = "noLastFatigueRecovery";
        PartnerMonsterId = "noIdA";
        PartnerMonsterIds = new string[] { };
    }
}


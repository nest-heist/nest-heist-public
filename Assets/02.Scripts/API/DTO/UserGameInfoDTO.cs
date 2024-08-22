using UnityEngine;

public struct UpdatePartnerMonsterBody
{
    public string userId;
    public string partnerMonsterId;
}

public struct UpdatePartyBody
{
    public string UserId;
    public string[] MonsterIds;
}

public struct UpdateFatigueBody
{
    public string UserId;
    public int Amount;
}

public struct LevelUpBody
{
    public string UserId;
    public int Level;
    public int Exp;
}

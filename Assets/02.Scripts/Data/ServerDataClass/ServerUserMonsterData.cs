using System;

[Serializable]
public class ServerUserMonsterData
{
    public string Id;
    public string UserId;
    public string MonsterId;
    // (처음 만들어줄 때 스탯에 더해주는 가중치, 서버에서 분포에 따라 계산해서 돌려준다 )
    public int IV;
    public int Level;
    public int EXP;
    public int Rank;
    public string SkillId;

    public ServerUserMonsterData()
    {
        Id = "noId";
        MonsterId = "noMId";
        IV = 0;
        Level = 0;
        EXP = 0;
        Rank = 0;
        SkillId = "noSkillId";
    }
}

public class ServerUserMonsterAndWinData : ServerUserMonsterData
{
    public bool IsWin;
}

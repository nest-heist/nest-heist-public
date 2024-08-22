using System;

/// <summary>
/// 서버랑 소통할 알 데이터 
/// </summary>
[Serializable]
public class ServerUserEggData : ItemData
{
    public string EggId;
    public string HatchStart;
    public bool IsHatched;

    public ServerUserEggData()
    {
        Id = "";
        UserId = "";
        EggId = "EGG00001";
        AcquiredAt = "";
        HatchStart = "";
        IsHatched = false;
    }
}
public class AddedServerUserEggData
{
    public ServerUserEggData AddedEgg;
}

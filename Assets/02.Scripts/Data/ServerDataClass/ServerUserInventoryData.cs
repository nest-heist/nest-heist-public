using System;

/// <summary>
/// 서버랑 동기화할 인벤토리 데이터
/// </summary>
[Serializable]
public class ServerUserInventoryData : ItemData
{
    // 아이템 타입 말고 ItemId로 찾기, 서버에서 값 받아오는 클래스라 바로 못 바꾼다 
    public Define.ItemType ItemType;
    public string ItemId;
    public int Quantity;

    public ServerUserInventoryData()
    {
        this.Id = "noId";
        this.UserId = "noUserId";
        this.ItemType = Define.ItemType.FreeCurrency;
        this.ItemId = "ITE00000";
        this.Quantity = 1;
        this.AcquiredAt = "noAcquired";
    }
}

/// <summary>
/// 아이템 - 인벤토리 + 알 데이터
/// 인벤토리에서 보여줄 때 갖고있을 상위 계층이 필요해서 만들었다 
/// </summary>
public class ItemData
{
    public string Id;
    public string UserId;
    public string AcquiredAt;
}
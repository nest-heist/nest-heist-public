using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 인벤토리에 들어가는 것들
/// 골드, 다이아, 초급영양제, 중급영양제, 고급영양제
/// </summary>
public class ItemReward : Reward
{
    public ItemReward(RewardInfoData data) : base(data)
    {
        Iconsprite = ResourceManager.Instance.Load<Sprite>($"Icon/ItemIcon/{RewardInfo.RewardItemId}");
    }

    public override void Give(Quest quest)
    {
        try
        {
            base.Give(quest);

            List<ServerUserInventoryData> userInventoryList = DataManager.Instance.ServerDataSystem.UserInventoryList;
            ServerUserInventoryData item = userInventoryList.Find(x => x.ItemId == RewardInfo.RewardItemId);
            // 이미 존재하는 아이템이면 수량 증가
            if (item != null)
            {
                item.Quantity += RewardInfo.Quantity;
            }
            // 존재하지 않으면 새로운 인벤토리 데이터 추가
            else
            {
                ServerUserInventoryData newItem = new ServerUserInventoryData
                {
                    ItemId = RewardInfo.RewardItemId,
                    Quantity = RewardInfo.Quantity
                };
                userInventoryList.Add(newItem);
            }
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 경험치
/// 획득하면 바로 플레이어 경험치에 더해진다
/// </summary>
public class ExpReward : Reward
{
    public ExpReward(RewardInfoData data) : base(data)
    {
        Iconsprite = ResourceManager.Instance.Load<Sprite>($"Icon/PlayerIcon/{RewardInfo.RewardItemId}");
    }

    /// <summary>
    /// </summary>
    /// <param name="quest"></param>
    public override void Give(Quest quest)
    { 
        try
        {
            base.Give(quest);
            // 경험치 주기 이거 서버에다가 퀘스트완료 + 리워드 + 경험치로 한번에 처리하는게 맞는 것 같다
        }
        catch (Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }
}

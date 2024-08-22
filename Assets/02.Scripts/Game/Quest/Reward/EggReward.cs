using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 알
/// 몬스터 알 인벤토리에 더해진다
/// </summary>
public class EggReward : Reward
{
    public EggReward(RewardInfoData data) : base(data)
    {
        Iconsprite = ResourceManager.Instance.LoadSprite("Icon/EggIcon/eggs", RewardInfo.RewardItemId);
    }

    /// <summary>
    /// 알 리워드는 서버에서 알 아이디 만들어서 그 때 알 인벤토리에 넣어준다
    /// </summary>
    /// <param name="quest"></param>
    public override void Give(Quest quest)
    {

    }
}
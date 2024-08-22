using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 타입마다 아이콘 스프라이트 경로 달라져서 생성할 때 아이콘 스프라이트를 같이 넣어준다
/// </summary>
public class Reward
{
    public RewardInfoData RewardInfo { get; set; }
    public Sprite Iconsprite { get; set; }

    public Reward(RewardInfoData data)
    {
        RewardInfo = data;
    }

    public virtual void Give(Quest quest)
    {
    }
}

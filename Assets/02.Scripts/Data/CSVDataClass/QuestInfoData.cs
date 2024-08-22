
using System.Collections.Generic;
/// <summary>
/// 퀘스트 데이터
/// </summary>
public class QuestInfoData
{
    // 퀘스트 아이디
    public string Id;

    // 퀘스트 이름
    public string Name;

    // 퀘스트 설명
    public string Desc;

    // 퀘스트 카테고리
    public Define.QuestCategory Category;

    // 퀘스트 하위 태스크 - SubtaskInfoData 먼저 만들고 아이디로 찾아서 넣어주기
    public Dictionary<string, SubtaskInfoData> TaskDic;

    // 퀘스트 보상 - RewardInfoData 먼저 만들고 아이디로 찾아서 넣어주기
    public Dictionary<string, RewardInfoData> RewardDic;
}


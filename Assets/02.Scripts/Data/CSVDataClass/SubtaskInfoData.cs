
using System.Collections.Generic;

/// <summary>
/// 테스크 데이터
/// </summary>
public class SubtaskInfoData
{
    // 태스크 아이디
    public string Id;

    // 태스크를 들고있는 퀘스트 아이디
    public string QuestId;

    // 태스크 설명
    public string Desc;

    // 하나의 태스크에 여러 타겟 카운팅 가능 ex) 빨강 노랑 슬라임 10마리 잡기
    public List<SubtaskTarget> TargetList;

    // 태스크 완료하려면 필요한 값
    public int NeedSuccessToCompleted;

    // 테스크 카운팅 방식
    public SuccessCountProcessor SuccessCountProcessor;

    /// <summary>
    /// 기본적으로 횟수 카운팅 해주는 걸로 넣기
    /// TODO : CSV 파싱해서 넣어주기
    /// </summary>
    public SubtaskInfoData()
    {
        SuccessCountProcessor = new SimpleCount();

        TargetList = new List<SubtaskTarget>();
    }
}



using UnityEngine;


/// <summary>
/// 현재 스텟을 계산하는 클래스
/// </summary>
public class UserMonster
{
    public MonsterAllStatData AllStatData { get; private set; }
    public ServerUserMonsterData UserData { get; private set; }
    // 파트너 여부를 나타내는 속성 추가
    public bool IsPartner { get; set; }

    public MonsterStat CurrentStat { get; private set; }
    public MonsterStat BaseStat;
    public MonsterStat IVStat;
    public MonsterStat RankStat;
    public MonsterStat LevelStat;

    public UserMonster(MonsterAllStatData allStatData, ServerUserMonsterData userData)
    {
        AllStatData = allStatData;
        UserData = userData;

        // 기본 스탯 초기화
        BaseStat = allStatData.BaseStat.ReturnStat().DeepCopy();
        CurrentStat = BaseStat.DeepCopy();

        // 초기 스탯 계산
        UpdateIVStat();
        UpdateRankStat();
        UpdateLevelStat();
    }

    public void UpdateIVStat()
    {
        IVStat = AllStatData.IVStat.ReturnStat().StatCalculator(StatCalculator.CalculateIVStat, UserData.IV);
        CurrentStat.Add(IVStat);
    }

    public void UpdateRankStat()
    {
        RankStat = AllStatData.RankStat.ReturnStat().StatCalculator(StatCalculator.CalculateRankStat, UserData.Rank);
        CurrentStat.Add(RankStat);
    }



    public void UpdateLevelStat()
    {
        LevelStat = AllStatData.LevelStat.ReturnStat().StatCalculator(StatCalculator.CalculateLevelStat, UserData.Level);
        CurrentStat.Add(LevelStat);
    }

    public void LevelUp()
    {
        // 레벨 증가
        UserData.Level++;
        // 레벨 스탯 재계산
        UpdateLevelStat();
    }

    public void IncreaseRank()
    {
        // 랭크 증가
        UserData.Rank++;
        // 랭크 스탯 재계산
        UpdateRankStat();
    }

    public void UpdateIV(int newIV)
    {
        // 새로운 IV 값 설정
        UserData.IV = newIV;
        // IV 스탯 재계산
        UpdateIVStat();
    }
}

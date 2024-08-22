using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 딕셔너리로 만들어진 CSV 데이터들을 읽기 전용으로 관리하는 클래스
/// </summary>
public class ReadOnlyDataSystem
{
    /// --- 던전 데이터 ---
    public Dictionary<string, List<DungeonEggProbabilityData>> DungeonEggProbability { get; private set; }
    public Dictionary<string, DungeonInfoData> DungeonInfo { get; private set; }
    public Dictionary<string, List<DungeonMonsterProbabilityData>> DungeonMonsterProbability { get; private set; }

    public Dictionary<string, EggInfoData> EggInfo { get; private set; }

    public Dictionary<string, ItemInfoData> ItemInfo { get; private set; }

    /// --- 몬스터 데이터 ---
    public Dictionary<string, MonsterStatData> MonsterBaseStat { get; private set; }
    public Dictionary<string, List<MonsterDropItemData>> MonsterDropItem { get; private set; }
    public Dictionary<string, MonsterInfoData> MonsterInfo { get; private set; }
    public Dictionary<string, MonsterStatData> MonsterIVStat { get; private set; }

    public Dictionary<string, MonsterStatData> MonsterLevelStat { get; private set; }
    public Dictionary<int, MonsterLevelUpData> MonsterLevelUp { get; private set; }
    public Dictionary<string, MonsterStatData> MonsterRankStat { get; private set; }
    public Dictionary<string, MonsterSkillInfoData> MonsterSkillInfo { get; private set; }
    public Dictionary<string, MonsterAllStatData> MonsterAllStat { get; private set; }


    /// --- 플레이어 데이터 ---
    public Dictionary<string, PlayerBaseStatData> PlayerBaseStat { get; private set; }
    public Dictionary<string, PlayerInfoData> PlayerInfo { get; private set; }
    public Dictionary<string, PlayerLevelStatData> PlayerLevelStat { get; private set; }
    public Dictionary<int, PlayerLevelUpData> PlayerLevelUp { get; private set; }
    public Dictionary<string, PlayerAllStatData> PlayerAllStat { get; private set; }


    /// --- 퀘스트 관련 데이터 ---
    public Dictionary<string, SubtaskInfoData> Task { get; private set; }
    public Dictionary<string, RewardInfoData> Reward { get; private set; }
    public Dictionary<string, QuestInfoData> Quest { get; private set; }

    private CSVLoader _csvLoader;

    public void Init()
    {
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _csvLoader = Object.FindObjectOfType<CSVLoader>();

            // 로더 없으면 프리팹 꺼내와서 씬에 만들어주기
            if (_csvLoader == null)
            {
                GameObject prefab = ResourceManager.Instance.Load<GameObject>("Prefabs/CSVLoader");
                GameObject loader = Object.Instantiate(prefab);
                loader.name = "Loader";
                _csvLoader = loader.GetComponent<CSVLoader>();
            }

            // ----- CSV 데이터 로드 시작 -----
            DungeonEggProbability = _csvLoader.MakeDungeonEggProbabilityData();
            DungeonInfo = _csvLoader.MakeDungeonInfoData();
            DungeonMonsterProbability = _csvLoader.MakeDungeonMonsterProbabilityData();

            EggInfo = _csvLoader.MakeEggInfoData();
            ItemInfo = _csvLoader.MakeItemInfoData();
            MonsterBaseStat = _csvLoader.MakeMonsterBaseStatData();
            MonsterDropItem = _csvLoader.MakeMonsterDropItemData();
            MonsterInfo = _csvLoader.MakeMonsterInfoData();
            MonsterIVStat = _csvLoader.MakeMonsterIVStatData();

            MonsterLevelStat = _csvLoader.MakeMonsterLevelStatData();
            MonsterLevelUp = _csvLoader.MakeMonsterLevelUpData();
            MonsterRankStat = _csvLoader.MakeMonsterRankStatData();
            MonsterSkillInfo = _csvLoader.MakeMonsterSkillInfoData();

            PlayerBaseStat = _csvLoader.MakePlayerBaseStatData();
            PlayerInfo = _csvLoader.MakePlayerInfoData();
            PlayerLevelStat = _csvLoader.MakePlayerLevelStatData();
            PlayerLevelUp = _csvLoader.MakePlayerLevelUpData();

            MonsterAllStat = MergeMonsterAllStat();
            PlayerAllStat = MergePlayerAllStat();

            Task = _csvLoader.MakeTaskInfoData();
            Reward = _csvLoader.MakeRewardInfoData();
            Quest = _csvLoader.MakeQuestInfoData();
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }

    /// <summary>
    /// CSV 데이터 다 불러오기 끝나고 불러온 데이터 한 번 더 정제하기
    /// MonsterAllStatData에 MonsterIdList 기준으로 모든 데이터 넣어주기
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, MonsterAllStatData> MergeMonsterAllStat()
    {
        Dictionary<string, MonsterAllStatData> dic = new Dictionary<string, MonsterAllStatData>();

        foreach (MonsterStatData monsterdata in MonsterBaseStat.Values)
        {
            string monsterId = monsterdata.MonsterId;

            MonsterStatData monsterBaseStat = MonsterBaseStat[monsterId];
            MonsterStatData monsterIVStatData = MonsterIVStat.ContainsKey(monsterId) ? MonsterIVStat[monsterId] : null;
            MonsterStatData monsterLevelStatData = MonsterLevelStat.ContainsKey(monsterId) ? MonsterLevelStat[monsterId] : null;
            MonsterStatData monsterRankStatData = MonsterRankStat.ContainsKey(monsterId) ? MonsterRankStat[monsterId] : null;

            MonsterAllStatData monsterAllStatData = new MonsterAllStatData()
            {
                BaseStat = monsterBaseStat,
                IVStat = monsterIVStatData,
                LevelStat = monsterLevelStatData,
                RankStat = monsterRankStatData,
            };

            dic.Add(monsterId, monsterAllStatData);
        }
        return dic;
    }

    private Dictionary<string, PlayerAllStatData> MergePlayerAllStat()
    {
        Dictionary<string, PlayerAllStatData> dic = new Dictionary<string, PlayerAllStatData>();

        foreach (PlayerBaseStatData playerStatData in PlayerBaseStat.Values)
        {
            string playerId = playerStatData.PlayerId;

            PlayerBaseStatData playerBaseStat = PlayerBaseStat[playerId];
            PlayerLevelStatData playerLvUpStatData = PlayerLevelStat.ContainsKey(playerId) ? PlayerLevelStat[playerId] : null;

            PlayerAllStatData playerAllStatData = new PlayerAllStatData()
            {
                BaseStat = playerBaseStat,
                LevelStat = playerLvUpStatData,
            };

            dic.Add(playerId, playerAllStatData);
        }
        return dic;
    }
}

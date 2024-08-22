using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using static Define;

/// <summary>
/// 데이터 로드해서 딕셔너리로 만들어주는 함수 갖고있는 로더 클래스
/// Loader에 컴포넌트로 붙혀놓는다.
/// private 이지만 파일 이름과 변수명 동일하게 쓰려고 _안 붙힌다.
/// </summary>
public class CSVLoader : MonoBehaviour
{
    private string _csvPath = "CSV";

    private string DungeonEggProbabilityData = "DungeonEggProbabilityData";
    private string DungeonInfoData = "DungeonInfoData";
    private string DungeonMonsterProbabilityData = "DungeonMonsterProbabilityData";

    private string EggInfoData = "EggInfoData";

    private string ItemInfoData = "ItemInfoData";

    private string MonsterBaseStatData = "MonsterBaseStatData";
    private string MonsterDropItemData = "MonsterDropItemData";
    private string MonsterInfoData = "MonsterInfoData";
    private string MonsterIVStatData = "MonsterIVStatData";

    private string MonsterLevelStatData = "MonsterLevelStatData";
    private string MonsterLevelUpData = "MonsterLevelUpData";
    private string MonsterRankStatData = "MonsterRankStatData";
    private string MonsterSkillInfoData = "MonsterSkillInfoData";

    private string PlayerBaseStatData = "PlayerBaseStatData";
    private string PlayerInfoData = "PlayerInfoData";
    private string PlayerLevelStatData = "PlayerLevelStatData";
    private string PlayerLevelUpData = "PlayerLevelUpData";

    private string SubtaskInfoData = "SubtaskInfoData";
    private string RewardInfoData = "RewardInfoData";
    private string QuestInfoData = "QuestInfoData";

    public Dictionary<string, List<DungeonEggProbabilityData>> MakeDungeonEggProbabilityData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{DungeonEggProbabilityData}");

        Dictionary<string, List<DungeonEggProbabilityData>> dic = new Dictionary<string, List<DungeonEggProbabilityData>>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            DungeonEggProbabilityData data = new DungeonEggProbabilityData();

            data.DungeonId = _tempData[i]["DungeonId"].ToString();
            data.EggId = _tempData[i]["EggId"].ToString();
            data.Probability = float.Parse(_tempData[i]["Probability"].ToString());

            if (dic.ContainsKey(data.DungeonId))
            {
                dic[data.DungeonId].Add(data);
            }
            else
            {
                dic.Add(data.DungeonId, new List<DungeonEggProbabilityData> { data });
            }
        }

        if (dic == null)
        {
            Logging.LogError($"DungeonEggProbabilityData");
        }

        return dic;
    }

    public Dictionary<string, DungeonInfoData> MakeDungeonInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{DungeonInfoData}");

        Dictionary<string, DungeonInfoData> dic = new Dictionary<string, DungeonInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            DungeonInfoData data = new DungeonInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.EnvironmentType = (EnvironmentType)Enum.Parse(typeof(EnvironmentType), _tempData[i]["EnvironmentType"].ToString());
            data.Stage = int.Parse(_tempData[i]["Stage"].ToString());
            data.Fatigue = int.Parse(_tempData[i]["Fatigue"].ToString());
            data.MinLevel = int.Parse(_tempData[i]["MinLevel"].ToString());
            data.MaxLevel = int.Parse(_tempData[i]["MaxLevel"].ToString());
            data.DungeonWidth = int.Parse(_tempData[i]["DungeonWidth"].ToString());
            data.DungeonHeight = int.Parse(_tempData[i]["DungeonHeight"].ToString());
            data.ClearExp = int.Parse(_tempData[i]["ClearExp"].ToString());
            data.MonsterSpawnNum = int.Parse(_tempData[i]["MonsterSpawnNum"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"DungeonInfoData");
        }

        return dic;
    }

    public Dictionary<string, List<DungeonMonsterProbabilityData>> MakeDungeonMonsterProbabilityData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{DungeonMonsterProbabilityData}");

        Dictionary<string, List<DungeonMonsterProbabilityData>> dic = new Dictionary<string, List<DungeonMonsterProbabilityData>>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            DungeonMonsterProbabilityData data = new DungeonMonsterProbabilityData();

            data.DungeonId = _tempData[i]["DungeonId"].ToString();
            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.Probability = float.Parse(_tempData[i]["Probability"].ToString());

            if (dic.ContainsKey(data.DungeonId))
            {
                dic[data.DungeonId].Add(data);
            }
            else
            {
                dic.Add(data.DungeonId, new List<DungeonMonsterProbabilityData> { data });
            }
        }

        if (dic == null)
        {
            Logging.LogError($"DungeonMonsterProbabilityData");
        }

        return dic;
    }

    public Dictionary<string, EggInfoData> MakeEggInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{EggInfoData}");

        Dictionary<string, EggInfoData> dic = new Dictionary<string, EggInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            EggInfoData data = new EggInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Name = _tempData[i]["Name"].ToString();

            string temp = _tempData[i]["MonsterIdList"].ToString();

            // 슬라임 알
            if (data.Id == "EGGSLM01")
            {
                // 슬라임 몬스터 리스트 생성
                List<string> slimeMonsterList = new List<string>();
                for (int j = 101; j <= 150; j++)
                {
                    slimeMonsterList.Add($"MON00{j}");
                }
                data.MonsterIdList = slimeMonsterList;
            }
            // 슬라임 아닌 일반 알
            else
            {
                data.MonsterIdList = new List<string>(temp.Split(','));
            }

            data.EnvironmentType = (EnvironmentType)Enum.Parse(typeof(EnvironmentType), _tempData[i]["EnvironmentType"].ToString());
            data.HatchTime = int.Parse(_tempData[i]["HatchTime"].ToString());
            data.NeedGauge = int.Parse(_tempData[i]["NeedGauge"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"EggInfoData");
        }

        return dic;
    }

    public Dictionary<string, ItemInfoData> MakeItemInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{ItemInfoData}");

        Dictionary<string, ItemInfoData> dic = new Dictionary<string, ItemInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            ItemInfoData data = new ItemInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Name = _tempData[i]["Name"].ToString();
            data.Desc = _tempData[i]["Desc"].ToString();
            data.Type = (ItemType)Enum.Parse(typeof(ItemType), _tempData[i]["Type"].ToString());
            data.Value = int.Parse(_tempData[i]["Value"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"ItemInfoData");
        }

        return dic;
    }

    public Dictionary<string, MonsterStatData> MakeMonsterBaseStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterBaseStatData}");

        Dictionary<string, MonsterStatData> dic = new Dictionary<string, MonsterStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterStatData data = new MonsterStatData();

            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.Attack = int.Parse(_tempData[i]["Attack"].ToString());
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Defence = int.Parse(_tempData[i]["Defence"].ToString());
            data.Critical = float.Parse(_tempData[i]["Critical"].ToString());
            data.CriticalDamage = int.Parse(_tempData[i]["CriticalDamage"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.MonsterId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterBaseStatData");
        }

        return dic;
    }

    public Dictionary<string, List<MonsterDropItemData>> MakeMonsterDropItemData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterDropItemData}");

        Dictionary<string, List<MonsterDropItemData>> dic = new Dictionary<string, List<MonsterDropItemData>>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterDropItemData data = new MonsterDropItemData();

            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.ItemId = _tempData[i]["ItemId"].ToString();
            data.Probability = float.Parse(_tempData[i]["Probability"].ToString());

            // 그냥 작업하다가 MinMax int를 안 넣었다. 그리고 디버깅하면서 안나온다고 했다...ㅎ
            data.MinNum = int.Parse(_tempData[i]["MinNum"].ToString());
            data.MaxNum = int.Parse(_tempData[i]["MaxNum"].ToString());


            if (dic.ContainsKey(data.MonsterId))
            {
                dic[data.MonsterId].Add(data);
            }
            else
            {
                dic.Add(data.MonsterId, new List<MonsterDropItemData> { data });
            }
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterDropItemData");
        }

        return dic;
    }

    public Dictionary<string, MonsterInfoData> MakeMonsterInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterInfoData}");

        Dictionary<string, MonsterInfoData> dic = new Dictionary<string, MonsterInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterInfoData data = new MonsterInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Name = _tempData[i]["Name"].ToString();
            data.Desc = _tempData[i]["Desc"].ToString();

            string temp = _tempData[i]["AttributeType"].ToString();
            List<string> strings = new List<string>(temp.Split(','));
            foreach (string type in strings)
            {
                switch (type)
                {
                    case "물":
                        data.AttributeTypeList.Add(AttributeType.Water);
                        break;
                    case "불":
                        data.AttributeTypeList.Add(AttributeType.Fire);
                        break;
                    case "풀":
                        data.AttributeTypeList.Add(AttributeType.Grass);
                        break;
                    case "땅":
                        data.AttributeTypeList.Add(AttributeType.Ground);
                        break;
                    case "얼음":
                        data.AttributeTypeList.Add(AttributeType.Ice);
                        break;
                    case "전기":
                        data.AttributeTypeList.Add(AttributeType.Electric);
                        break;
                    case "드래곤":
                        data.AttributeTypeList.Add(AttributeType.Dragon);
                        break;
                    case "어둠":
                        data.AttributeTypeList.Add(AttributeType.Dark);
                        break;
                    case "노말":
                        data.AttributeTypeList.Add(AttributeType.Normal);
                        break;
                    default:
                        Logging.LogError($"MonsterInfoData AttributeType Error");
                        break;
                }
            }

            data.AttackType = (AttackType)Enum.Parse(typeof(AttackType), _tempData[i]["AttackType"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterInfoData");
        }

        return dic;
    }

    public Dictionary<string, MonsterStatData> MakeMonsterIVStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterIVStatData}");

        Dictionary<string, MonsterStatData> dic = new Dictionary<string, MonsterStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterStatData data = new MonsterStatData();

            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.Attack = int.Parse(_tempData[i]["Attack"].ToString());
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Defence = int.Parse(_tempData[i]["Defence"].ToString());
            data.Critical = float.Parse(_tempData[i]["Critical"].ToString());
            data.CriticalDamage = int.Parse(_tempData[i]["CriticalDamage"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.MonsterId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterIVStatData");
        }

        return dic;
    }

    public Dictionary<string, MonsterStatData> MakeMonsterLevelStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterLevelStatData}");

        Dictionary<string, MonsterStatData> dic = new Dictionary<string, MonsterStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterStatData data = new MonsterStatData();

            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.Attack = int.Parse(_tempData[i]["Attack"].ToString());
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Defence = int.Parse(_tempData[i]["Defence"].ToString());
            data.Critical = float.Parse(_tempData[i]["Critical"].ToString());
            data.CriticalDamage = int.Parse(_tempData[i]["CriticalDamage"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.MonsterId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterLevelStatData");
        }

        return dic;
    }

    public Dictionary<int, MonsterLevelUpData> MakeMonsterLevelUpData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterLevelUpData}");

        Dictionary<int, MonsterLevelUpData> dic = new Dictionary<int, MonsterLevelUpData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterLevelUpData data = new MonsterLevelUpData();

            data.Level = int.Parse(_tempData[i]["Level"].ToString());
            data.SumOfExp = int.Parse(_tempData[i]["SumOfExp"].ToString());

            dic.Add(data.Level, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterLevelUpData");
        }

        return dic;
    }

    public Dictionary<string, MonsterStatData> MakeMonsterRankStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterRankStatData}");

        Dictionary<string, MonsterStatData> dic = new Dictionary<string, MonsterStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterStatData data = new MonsterStatData();

            data.MonsterId = _tempData[i]["MonsterId"].ToString();
            data.Attack = int.Parse(_tempData[i]["Attack"].ToString());
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Defence = int.Parse(_tempData[i]["Defence"].ToString());
            data.Critical = float.Parse(_tempData[i]["Critical"].ToString());
            data.CriticalDamage = int.Parse(_tempData[i]["CriticalDamage"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.MonsterId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterRankStatData");
        }

        return dic;
    }

    public Dictionary<string, MonsterSkillInfoData> MakeMonsterSkillInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{MonsterSkillInfoData}");

        Dictionary<string, MonsterSkillInfoData> dic = new Dictionary<string, MonsterSkillInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            MonsterSkillInfoData data = new MonsterSkillInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Name = _tempData[i]["Name"].ToString();
            data.Desc = _tempData[i]["Desc"].ToString();
            data.Range = float.Parse(_tempData[i]["Range"].ToString());
            data.CoolDown = float.Parse(_tempData[i]["CoolDown"].ToString());
            data.CastTime = float.Parse(_tempData[i]["CastTime"].ToString());
            data.DamageMulti = float.Parse(_tempData[i]["DamageMulti"].ToString());

            string type = _tempData[i]["AttributeType"].ToString();
            switch (type)
            {
                case "물":
                    data.AttributeType = AttributeType.Water;
                    break;
                case "불":
                    data.AttributeType = AttributeType.Fire;
                    break;
                case "풀":
                    data.AttributeType = AttributeType.Grass;
                    break;
                case "땅":
                    data.AttributeType = AttributeType.Ground;
                    break;
                case "얼음":
                    data.AttributeType = AttributeType.Ice;
                    break;
                case "전기":
                    data.AttributeType = AttributeType.Electric;
                    break;
                case "드래곤":
                    data.AttributeType = AttributeType.Dragon;
                    break;
                case "어둠":
                    data.AttributeType = AttributeType.Dark;
                    break;
                case "노말":
                    data.AttributeType = AttributeType.Normal;
                    break;
                default:
                    Logging.LogError($"MonsterSkillInfoData AttributeType Error");
                    break;
            }

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"MonsterSkillInfoData");
        }

        return dic;
    }

    public Dictionary<string, PlayerBaseStatData> MakePlayerBaseStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{PlayerBaseStatData}");

        Dictionary<string, PlayerBaseStatData> dic = new Dictionary<string, PlayerBaseStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            PlayerBaseStatData data = new PlayerBaseStatData();

            data.PlayerId = _tempData[i]["PlayerId"].ToString();
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Avoid = float.Parse(_tempData[i]["Avoid"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.PlayerId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"PlayerBaseStatData");
        }

        return dic;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, PlayerInfoData> MakePlayerInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{PlayerInfoData}");

        Dictionary<string, PlayerInfoData> dic = new Dictionary<string, PlayerInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            PlayerInfoData data = new PlayerInfoData();

            data.Id = _tempData[i]["Id"].ToString();

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"PlayerInfoData");
        }

        return dic;
    }

    public Dictionary<string, PlayerLevelStatData> MakePlayerLevelStatData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{PlayerLevelStatData}");

        Dictionary<string, PlayerLevelStatData> dic = new Dictionary<string, PlayerLevelStatData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            PlayerLevelStatData data = new PlayerLevelStatData();

            data.PlayerId = _tempData[i]["PlayerId"].ToString();
            data.MaxHP = int.Parse(_tempData[i]["MaxHP"].ToString());
            data.Avoid = float.Parse(_tempData[i]["Avoid"].ToString());
            data.WalkSpeed = float.Parse(_tempData[i]["WalkSpeed"].ToString());

            dic.Add(data.PlayerId, data);
        }

        if (dic == null)
        {
            Logging.LogError($"PlayerLevelStatData");
        }

        return dic;
    }

    public Dictionary<int, PlayerLevelUpData> MakePlayerLevelUpData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{PlayerLevelUpData}");

        Dictionary<int, PlayerLevelUpData> dic = new Dictionary<int, PlayerLevelUpData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            PlayerLevelUpData data = new PlayerLevelUpData();

            data.Level = int.Parse(_tempData[i]["Level"].ToString());
            data.SumOfExp = int.Parse(_tempData[i]["SumOfExp"].ToString());

            dic.Add(data.Level, data);
        }

        if (dic == null)
        {
            Logging.LogError($"PlayerLevelUpData");
        }

        return dic;
    }

    public Dictionary<string, SubtaskInfoData> MakeTaskInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{SubtaskInfoData}");

        Dictionary<string, SubtaskInfoData> dic = new Dictionary<string, SubtaskInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            SubtaskInfoData data = new SubtaskInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Desc = _tempData[i]["Desc"].ToString();

            string temp = _tempData[i]["Targets"].ToString();
            List<string> strings = new List<string>(temp.Split(','));
            foreach (string target in strings)
            {
                SubtaskTarget subtaskTarget = new StringTarget(target);
                data.TargetList.Add(subtaskTarget);
            }

            data.NeedSuccessToCompleted = int.Parse(_tempData[i]["NeedSuccessToCompleted"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"SubtaskInfoData");
        }

        return dic;
    }

    public Dictionary<string, RewardInfoData> MakeRewardInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{RewardInfoData}");

        Dictionary<string, RewardInfoData> dic = new Dictionary<string, RewardInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            RewardInfoData data = new RewardInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.RewardItemId = _tempData[i]["RewardItemId"].ToString();
            data.Quantity = int.Parse(_tempData[i]["Quantity"].ToString());

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"RewardInfoData");
        }

        return dic;
    }

    public Dictionary<string, QuestInfoData> MakeQuestInfoData()
    {
        List<Dictionary<string, object>> _tempData = CSVReader.Read($"{_csvPath}/{QuestInfoData}");

        Dictionary<string, QuestInfoData> dic = new Dictionary<string, QuestInfoData>();

        for (int i = 0; i < _tempData.Count; i++)
        {
            QuestInfoData data = new QuestInfoData();

            data.Id = _tempData[i]["Id"].ToString();
            data.Name = _tempData[i]["Name"].ToString();
            data.Desc = _tempData[i]["Desc"].ToString();

            string category = _tempData[i]["Category"].ToString();
            if (category == "던전")
            {
                data.Category = QuestCategory.Dungeon;
            }
            else if (category == "성장")
            {
                data.Category = QuestCategory.Growth;
            }
            else if (category == "일일")
            {
                data.Category = QuestCategory.Daily;
            }
            else
            {
                Logging.LogError($"QuestInfoData Category Error");
            }

            data.TaskDic = new Dictionary<string, SubtaskInfoData>();

            string temp = _tempData[i]["SubtaskIds"].ToString();
            List<string> strings = new List<string>(temp.Split(','));
            foreach (string id in strings)
            {
                SubtaskInfoData taskdata = DataManager.Instance.ReadOnlyDataSystem.Task[id];
                taskdata.QuestId = data.Id;
                data.TaskDic.Add(taskdata.Id, taskdata);
            }

            data.RewardDic = new Dictionary<string, RewardInfoData>();
            string temp2 = _tempData[i]["RewardIds"].ToString();
            List<string> strings2 = new List<string>(temp2.Split(','));
            foreach (string id in strings2)
            {
                RewardInfoData rewarddata = DataManager.Instance.ReadOnlyDataSystem.Reward[id];
                rewarddata.QuestId = data.Id;
                data.RewardDic.Add(rewarddata.Id, rewarddata);
            }

            dic.Add(data.Id, data);
        }

        if (dic == null)
        {
            Logging.LogError($"QuestInfoData");
        }

        return dic;
    }
}

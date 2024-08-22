using System.Collections.Generic;
using UnityEngine;
using static Define;

/// <summary>
/// 던전 스테이지 화면에 대응하는 매니저
/// TODO 추후 플레이어가 선택한 스테이지 정보를 가지도록함(지금인 임시로 고정값)
/// </summary>
public class DungeonStageSystem
{
    public string SelectedStageId { get; private set; }
    public string BossSelectedId { get; set; }
    public string BossEggId { get; set; }

    public Dictionary<EnvironmentType, DungeonMapSettingsSO> MapSettingDict = new Dictionary<EnvironmentType, DungeonMapSettingsSO>();

    public Dictionary<string, DungeonInfoData> DungeonInfo => DataManager.Instance.ReadOnlyDataSystem.DungeonInfo;
    public Dictionary<string, List<DungeonMonsterProbabilityData>> DungeonMonsterProbability => DataManager.Instance.ReadOnlyDataSystem.DungeonMonsterProbability;
    public Dictionary<string, List<DungeonEggProbabilityData>> DungeonEggProbability => DataManager.Instance.ReadOnlyDataSystem.DungeonEggProbability;

    public void Init()
    {
        MapSettingDict.Add(EnvironmentType.Grassland, ResourceManager.Instance.Load<DungeonMapSettingsSO>("SO/GrasslandMapSetting"));
        MapSettingDict.Add(EnvironmentType.SnowField, ResourceManager.Instance.Load<DungeonMapSettingsSO>("SO/SnowFieldMapSetting"));
        MapSettingDict.Add(EnvironmentType.Beach, ResourceManager.Instance.Load<DungeonMapSettingsSO>("SO/BeachMapSetting"));
        MapSettingDict.Add(EnvironmentType.Desert, ResourceManager.Instance.Load<DungeonMapSettingsSO>("SO/DesertMapSetting"));
    }

    public void SetDungeonId(string dungeonId)
    {
        SelectedStageId = dungeonId;
    }
}
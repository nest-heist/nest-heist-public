using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;

/// <summary>
/// 던전 맵 생성 매니저
/// </summary>
public class DungeonMapSystem
{
    public BSPProceduralGeneration MapGenerator { get; private set; }
    private DungeonMapSettingsSO _mapSettings;

    public void GenerateMap()
    {
        _mapSettings = GameManager.Instance.DungeonStageSystem.MapSettingDict[DungeonBattleManager.Instance.InfoData.EnvironmentType];

        MapGenerator = new BSPProceduralGeneration(
            _mapSettings,
            DungeonBattleManager.Instance.MapGrid.transform,
            new Vector2Int(DungeonBattleManager.Instance.InfoData.DungeonWidth, DungeonBattleManager.Instance.InfoData.DungeonHeight));

        MapGenerator.Generate();
    }
}
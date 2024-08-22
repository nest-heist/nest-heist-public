using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class DungeonPhaseSystem
{
    public IDungeonPhase CurrentPhase;
    private Dictionary<string, IDungeonPhase> _phases;
    public event Action<string> OnPhaseChanged;
    private DungeonBattleManager _dungeonBattleManager;
    private GameObject _exitTeleportPrefab;


    public DungeonPhaseSystem(DungeonBattleManager dungeonBattleManager, GameObject exitTeleportPrefab)
    {
        _dungeonBattleManager = dungeonBattleManager;
        _exitTeleportPrefab = exitTeleportPrefab;
        _phases = new Dictionary<string, IDungeonPhase>();

        // 동적으로 페이즈 클래스를 등록
        RegisterPhases(dungeonBattleManager);
    }

    private void RegisterPhases(DungeonBattleManager dungeonBattleManager)
    {
        // 현재 어셈블리에서 IDungeonPhase를 구현한 클래스를 검색
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            if (typeof(IDungeonPhase).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            {
                try
                {
                    // 각 클래스의 인스턴스를 생성하고 딕셔너리에 추가
                    IDungeonPhase phaseInstance = (IDungeonPhase)Activator.CreateInstance(type, new object[] { dungeonBattleManager });
                    phaseInstance.OnPhaseChangeRequested += HandlePhaseChangeRequested;
                    _phases.Add(type.Name, phaseInstance);
                }
                catch (Exception e)
                {
                    string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    string uid = DataManager.Instance.ServerDataSystem.User.UserId;
                    FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
                    Logging.LogError(e.Message);
                }
            }
        }
    }
    private void HandlePhaseChangeRequested(string nextPhase)
    {
        ChangePhase(nextPhase);
    }

    public void ChangePhase(string phaseName)
    {
        if (CurrentPhase != null)
        {
            CurrentPhase.ExitPhase();
        }


        if (_phases.ContainsKey(phaseName))
        {
            CurrentPhase = _phases[phaseName];
            CurrentPhase.EnterPhase();
        }
        else
        {
            Logging.LogError($"Phase {phaseName} not found.");
        }


    }
    public void ExitCurrentPhase()
    {
        if (CurrentPhase != null)
        {
            CurrentPhase.ExitPhase();
            CurrentPhase = null;
        }
    }

    public void UpdatePhase()
    {
        CurrentPhase?.UpdatePhase();
    }

    public void GenerateExitTeleport()
    {
        // 방 목록에서 랜덤한 방을 선택하여 탈출 텔레포트를 배치
        List<RectInt> rooms = _dungeonBattleManager.MapSystem.MapGenerator.GetRooms();
        RectInt exitRoom = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        // 방의 내부에서 랜덤한 위치를 선택
        int randomX = UnityEngine.Random.Range(exitRoom.xMin, exitRoom.xMax);
        int randomY = UnityEngine.Random.Range(exitRoom.yMin, exitRoom.yMax);
        // Vector3Int exitPosition = new Vector3Int(randomX, randomY, 0);
        Vector3 exitPosition = DungeonBattleManager.Instance.MapSystem.MapGenerator.GetSpawnPosition().ExitPos;


        try
        {
            GameObject exitTeleport = UnityEngine.Object.Instantiate(_exitTeleportPrefab, exitPosition, Quaternion.identity);
            exitTeleport.GetComponent<ExitTeleport>().IsActive = true;
            DungeonBattleManager.Instance.Player.GetComponent<OrbitAroundPlayer>().Init(exitTeleport.transform);
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

public interface IDungeonPhase
{
    event Action<string> OnPhaseChangeRequested;
    void EnterPhase();
    void UpdatePhase();
    void ExitPhase();
}

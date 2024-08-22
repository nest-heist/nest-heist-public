using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NavMeshPlus.Components;
using UnityEngine;

/// <summary>
/// 던전 진행 매니저
/// 스폰, 맵생성등의 매니저 자식으로 관리
/// </summary>
public class DungeonBattleManager : Singleton<DungeonBattleManager>, IBattleManager
{
    // 하위 매니저
    public DungeonMapSystem MapSystem { get; private set; }
    public DungeonSpawnSystem SpawnSystem { get; private set; }
    public DungeonPhaseSystem PhaseSystem { get; private set; }
    public DungeonDropSystem DropSystem { get; private set; }
    public DungeonSwitchingSystem SwitchingSystem { get; private set; }
    // 기본 데이터
    public DungeonInfoData InfoData { get; private set; }
    public List<DungeonMonsterProbabilityData> MonsterProbabilityData { get; private set; }
    public List<DungeonEggProbabilityData> EggProbabilityData { get; private set; }

    // Inspector
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public Grid MapGrid { get; private set; }
    [field: SerializeField] public NavMeshSurface NavMeshSurface { get; private set; }

    // 데이터
    public GameObject Player { get; private set; }
    private Player _player;
    public List<GameObject> EnemyMonsterList { get; private set; }

    // 모든 파트너 몬스터들과 알맞는 컴포넌트를 저장할 리스트
    public List<GameObject> AllPartnerMonster { get; private set; }
    private List<BaseMonster> _partnerMonstersBaseComponents = new List<BaseMonster>();

    public event Action OnLoaded;

    public UIScene_Dungeon DungeonScene;
    public bool CarryEgg;
    public string GetEggIdSet;
    public GameObject Egg;

    // TODO : 임시로 탈출 텔포 위치 추후 수정
    public GameObject ExitTeleportPrefab;
    public int PhaseCount;

    private int _originIndex;
    protected override async void Start()
    {
        // 던전에서 바로 테스트를 위한 데이터 로드
        await LoadingManager.Instance.RunWithLoading(DataManager.Instance.ServerDataSystem.InitData, onLoaded: () =>
        {
            GameManager.Instance.Load();
            GameManager.Instance.UserMonsterSystem.AllPartnerMonsterDie = false;
            Init();
            OnLoaded?.Invoke();

            UIManager.Instance.InstanciateUIScene<UIScene_Dungeon>();
            DungeonScene = FindObjectOfType<UIScene_Dungeon>();
            DungeonScene.SetBattleManager(this);
            SwitchingSystem.Init(DungeonScene);
        });

    }

    void Init()
    {
        InfoData = DataManager.Instance.ReadOnlyDataSystem.DungeonInfo[GameManager.Instance.DungeonStageSystem.SelectedStageId];
        EggProbabilityData = DataManager.Instance.ReadOnlyDataSystem.DungeonEggProbability[GameManager.Instance.DungeonStageSystem.SelectedStageId];
        MonsterProbabilityData = DataManager.Instance.ReadOnlyDataSystem.DungeonMonsterProbability[GameManager.Instance.DungeonStageSystem.SelectedStageId];
        AllPartnerMonster = new List<GameObject>();

        MapSystem = new DungeonMapSystem();
        SpawnSystem = new DungeonSpawnSystem(this, true);
        PhaseSystem = new DungeonPhaseSystem(this, ExitTeleportPrefab);
        PhaseSystem.OnPhaseChanged += HandlePhaseChanged;
        DropSystem = new DungeonDropSystem(this);
        SwitchingSystem = new DungeonSwitchingSystem(this);
        _originIndex = 0;

        StartInit();
    }

    private void StartInit()
    {
        try
        {
            MapSystem.GenerateMap();
            NavMeshSurface.BuildNavMeshAsync();


            InitializeAllPartnerMonsters();
            SpawnAtPosition();
            InitMonsters(AllPartnerMonster);
            PhaseSystem.ChangePhase("PhaseOne");
            AudioManager.Instance.PlayBGM("track_shortadventure_loop");
        }
        catch (Exception e)
        {
            //Logging.LogError($"Exception occurred during initialization: {e.Message}");
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            FirebaseManager.Instance.DBSystem.SendErrorLog(DataManager.Instance.ServerDataSystem.User.UserId, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }


    // 첫 초기화 때 모든 몬스터를 생성
    private void InitializeAllPartnerMonsters()
    {
        Vector3 spawnPosition = MapSystem.MapGenerator.GetSpawnPosition().PlayerSpawnPos;
        for (int i = 0; i < DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds.Length; i++)
        {
            GameObject partnerMonster = SpawnSystem.SpawnPartner(spawnPosition, i);
            partnerMonster.SetActive(false);
            AllPartnerMonster.Add(partnerMonster);
            _partnerMonstersBaseComponents.Add(partnerMonster.GetComponent<BaseMonster>());
        }
        AllPartnerMonster[0].SetActive(true);
    }

    private void InitMonsters(List<GameObject> monsters)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].GetComponent<BaseMonster>().Player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }
    }

    private void HandlePhaseChanged(string newPhase)
    {
        // Debug.Log($"페이즈가 {newPhase}(으)로 전환되었습니다.");
        // 페이즈 전환 시 필요한 추가 작업을 여기에 작성
    }

    private void SpawnAtPosition()
    {
        AllPartnerMonster[0].SetActive(true);
        Player = SpawnSystem.SpawnPlayer(MapSystem.MapGenerator.GetSpawnPosition().PlayerSpawnPos, AllPartnerMonster[0]);
        _player = Player.GetComponent<Player>();
        EnemyMonsterList = SpawnSystem.SpawnEnemy(MapSystem.MapGenerator.GetRooms());
        InitMonsters(EnemyMonsterList);
        // 몬스터 알 스폰 추가
        Egg = SpawnSystem.SpawnMonsterEgg(MapSystem.MapGenerator.GetRooms(), InfoData.Id);

        Player.GetComponent<OrbitAroundPlayer>().Init(Egg.transform);
    }

    public void SpawnPartnerMonster(int index)
    {
        if (AllPartnerMonster != null)
        {
            _partnerMonstersBaseComponents[_originIndex].SkillHandler.SaveCooldownState();
            AllPartnerMonster[_originIndex].SetActive(false);
        }

        _originIndex = index;
        AllPartnerMonster[index].transform.position = Player.transform.position;
        AllPartnerMonster[index].SetActive(true);
        _player.Partner = AllPartnerMonster[index];

        _partnerMonstersBaseComponents[index].SkillHandler.LoadCooldownState();

        string newMonsterId = SwitchingSystem.GetCurrentPartnerId();
        float savedHealth = SwitchingSystem.GetSavedHealth(newMonsterId);

        if (savedHealth != 1f)
        {
            StartCoroutine(SetHealthAfterInitialization(_partnerMonstersBaseComponents[index], savedHealth));
        }
    }

    private IEnumerator SetHealthAfterInitialization(BaseMonster partnerMonster, float savedHealth)
    {
        yield return null;

        var healthSystem = partnerMonster.HealthSystem;
        healthSystem.SetCurrentHealth((int)savedHealth);
        DungeonScene.Presenter.UpdatePartyHPBarCallBack();
    }
}

public interface IBattleManager
{
    GameObject Player { get; }
    void SpawnPartnerMonster(int index);
    DungeonSwitchingSystem SwitchingSystem { get; }
    DungeonInfoData InfoData { get; }
    List<DungeonMonsterProbabilityData> MonsterProbabilityData { get; }
    CinemachineVirtualCamera VirtualCamera { get; }
    DungeonDropSystem DropSystem { get; }
    List<GameObject> AllPartnerMonster { get; }
}
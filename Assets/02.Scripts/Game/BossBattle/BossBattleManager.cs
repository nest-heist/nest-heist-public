using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 던전 진행 매니저
/// 스폰, 맵생성등의 매니저 자식으로 관리
/// </summary>
public class BossBattleManager : Singleton<BossBattleManager>, IBattleManager
{
    public DungeonSpawnSystem SpawnSystem { get; private set; }

    // Inspector
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public Grid MapGrid { get; private set; }

    // 데이터
    public GameObject Player { get; private set; }
    public Player PlayerCs;

    public GameObject PartnerMonster { get; private set; }

    public DungeonSwitchingSystem SwitchingSystem { get; private set; }

    public DungeonInfoData InfoData { get; private set; }

    public List<DungeonMonsterProbabilityData> MonsterProbabilityData { get; private set; }

    public DungeonDropSystem DropSystem { get; private set; }

    public List<GameObject> AllPartnerMonster { get; private set; }
    private List<BaseMonster> _partnerMonstersBaseComponents = new List<BaseMonster>();

    public event Action OnLoaded;

    public UIScene_Boss BossScene;

    public GameObject Boss;
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

            UIManager.Instance.InstanciateUIScene<UIScene_Boss>();
            BossScene = FindObjectOfType<UIScene_Boss>();
            BossScene.SetBattleManager(this);

            SwitchingSystem.Init(BossScene);
        });

    }

    void Init()
    {
        AllPartnerMonster = new List<GameObject>();

        SpawnSystem = new DungeonSpawnSystem(this, false);
        SwitchingSystem = new DungeonSwitchingSystem(this);
        DropSystem = new DungeonDropSystem(this);

        _originIndex = 0;

        StartInit();
    }

    private void StartInit()
    {
        InitializeAllPartnerMonsters();
        SpawnAtPosition();
        InitMonsters(AllPartnerMonster);

        Boss.SetActive(true);
        Boss.GetComponent<Boss>().Init(PlayerCs);
    }

    private void SpawnAtPosition()
    {
        AllPartnerMonster[0].SetActive(true);

        Player = SpawnSystem.SpawnPlayer(MapGrid.transform.position, AllPartnerMonster[0]);
        PlayerCs = Player.GetComponent<Player>();
    }

    private void InitializeAllPartnerMonsters()
    {
        Vector3 spawnPosition = MapGrid.transform.position;
        for (int i = 0; i < DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds.Length; i++)
        {
            GameObject partnerMonster = SpawnSystem.SpawnPartner(spawnPosition, i);
            partnerMonster.SetActive(false);
            AllPartnerMonster.Add(partnerMonster);
            _partnerMonstersBaseComponents.Add(partnerMonster.GetComponent<BaseMonster>());
        }
    }
    private void InitMonsters(List<GameObject> monsters)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].GetComponent<BaseMonster>().Player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }
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
        PlayerCs.Partner = AllPartnerMonster[index];

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
        BossScene.Presenter.UpdatePartyHPBarCallBack();
    }
}

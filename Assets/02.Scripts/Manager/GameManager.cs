using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임진행하며 쭉 살아있는 매니저, 하위 매니저들을 들고있다.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public UserMonsterSystem UserMonsterSystem { get; private set; }
    public HatchSystem HatchingSystem { get; private set; }
    public DungeonStageSystem DungeonStageSystem { get; private set; }
    public QuestSystem QuestSystem { get; private set; }
    public FatigueSystem FatigueSystem { get; private set; }
    public UserLevelUpSystem UserSystem { get; private set; }
    public MonsterAttributeSystem MonsterAttributeSystem {  get; private set; }
    public UserInfoSystem UserInfoSystem { get; private set; }
    public PageTutorialSystem PageTutorialSystem { get; private set; }
    public ClickTutorialSystem ClickTutorialSystem { get; private set; }

    /// <summary>
    /// Awake 순서 확인하기 DataManager -> GameManager
    /// </summary>
    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// DataManager에서 데이터 로드 완료되면 호출
    /// </summary>
    public void Load()
    {
        if (UserMonsterSystem != null)
        {
            // 피로도 넘기는 테스트
            return;
        }

        // 하위 매니저 초기화
        UserMonsterSystem = new UserMonsterSystem();
        UserMonsterSystem.Init();

        DungeonStageSystem = new DungeonStageSystem();
        DungeonStageSystem.Init();

        HatchingSystem = new HatchSystem();
        HatchingSystem.Init();

        QuestSystem = new QuestSystem();
        QuestSystem.Init();

        FatigueSystem = new FatigueSystem();
        FatigueSystem.Init();

        UserSystem = new UserLevelUpSystem();
        UserSystem.Init();

        MonsterAttributeSystem = new MonsterAttributeSystem();

        UserInfoSystem = new UserInfoSystem();
        UserInfoSystem.Init();

        PageTutorialSystem = new PageTutorialSystem();

        ClickTutorialSystem = new ClickTutorialSystem();
        ClickTutorialSystem.Init();

        SetTargetFrame();
    }

    /// <summary>
    /// 모바일 쓰로클링 방지
    /// </summary>
    private void SetTargetFrame()
    {
        Application.targetFrameRate = 30;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.DestoryAllUIPopup();
        RandomExtension.InitStateFromTicks();
    }
}

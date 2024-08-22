using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_DungeonSelectStage : UIPopup
{
    [Header("TransForm")]
    public Transform StageListContainer;

    [Header("Button")]
    public Button ExitBtn;

    [Header("Script")]
    public UISubItem_DungeonSelectButton stageButtonPrefab;
    public UIScene_DungeonSelectStage_Presneter Presenter;

    [Header("Image")]
    public Image TypeImage;

    public string CurrentDungeonId;
    public int CurrentEnvironmentType { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIScene_DungeonSelectStage_Presneter(this);
    }

    protected override void Start()
    {
        base.Start();
        Presenter.Init();
    }

    public void CreateStageButton(DungeonInfoData dungeonInfo, bool isUnlocked)
    {
        // 스테이지 버튼 생성 및 설정
        UISubItem_DungeonSelectButton stageButton = Instantiate(stageButtonPrefab, StageListContainer);
        stageButton.Initialize(dungeonInfo, isUnlocked);
    }

    public void SetButton(Action onExit)
    {
        ExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
    }
}

public class UIScene_DungeonSelectStage_Presneter
{
    private UIPopup_DungeonSelectStage _view;
    private Dictionary<string, DungeonInfoData> _dungeonInfoDic;
    private List<ServerUserDungeonProgressData> _userDungeonProgressList;

    public UIScene_DungeonSelectStage_Presneter(UIPopup_DungeonSelectStage view)
    {
        _view = view;
        _userDungeonProgressList = DataManager.Instance.ServerDataSystem.UserDungeonProgressList;
    }

    public void Init()
    {
        SetButton();
        InitializeDungeonInfoDic();
        DisplayDungeonStages();
        SetImage();
    }

    private void InitializeDungeonInfoDic()
    {
        if (_dungeonInfoDic == null)
        {
            _dungeonInfoDic = DataManager.Instance.ReadOnlyDataSystem.DungeonInfo;
        }
    }

    public void DisplayDungeonStages()
    {
        // 딕셔너리가 초기화되었는지 확인
        if (_dungeonInfoDic == null || _dungeonInfoDic.Count == 0)
        {
            InitializeDungeonInfoDic();
            if (_dungeonInfoDic == null || _dungeonInfoDic.Count == 0)
            {
                return;
            }
        }

        // 기존에 배치된 버튼들을 모두 제거
        foreach (Transform child in _view.StageListContainer)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Dictionary에서 해당 EnvironmentType의 던전 정보를 필터링
        foreach (var kvp in _dungeonInfoDic)
        {
            if ((int)kvp.Value.EnvironmentType == _view.CurrentEnvironmentType)
            {
                bool isUnlocked = CheckIfStageIsUnlocked(kvp.Value.Id);
                _view.CreateStageButton(kvp.Value, isUnlocked);
            }
        }
    }

    // 스테이지 잠김 여부를 확인하는 메서드
    private bool CheckIfStageIsUnlocked(string dungeonId)
    {
        // 스테이지 정보에서 ID와 타입을 가져옴
        var dungeonInfo = _dungeonInfoDic[dungeonId];
        int stageNumber = dungeonInfo.Stage;
        var environmentType = dungeonInfo.EnvironmentType;

        // 첫 번째 스테이지는 항상 열려 있음
        if (stageNumber == 11)
        {
            return true;
        }

        // 현재 스테이지보다 낮은 스테이지는 모두 클리어된 것으로 간주하여 잠금 해제
        var maxClearedStageData = _userDungeonProgressList.FindLast(data => data.EnvironmentType == environmentType);

        if (maxClearedStageData != null && maxClearedStageData.StageId >= stageNumber)
        {
            return true;
        }

        return false;
    }

    private void SetImage()
    {
        Sprite sprite = ResourceManager.Instance.Load<Sprite>($"Icon/EnvironmentTypeIcon/{_view.CurrentEnvironmentType}");
        _view.TypeImage.sprite = sprite;
    }

    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopup_DungeonEnterCheck : UIPopup
{
    [Header("ImageContainer")]
    public Transform ApearMonsterContainer;
    public Transform DropItemContainer;
    public Transform DropEggContainer;

    [Header("Text")]
    public TextMeshProUGUI DropExp;
    public TextMeshProUGUI ApearMonsterAndLevel;
    public TextMeshProUGUI FatigueCost;
    public TextMeshProUGUI DungeonTitle;

    [Header("Button")]
    public Button EnterBtn;
    public Button ExitBtn;
    public Button AreaOutBtn;

    public UIPopup_DungeonEnterCheck_Presenter Presenter;
    public string DungeonId;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        Presenter = new UIPopup_DungeonEnterCheck_Presenter(this);
        Presenter.Init();
    }

    public void SetButton(Action onEnter, Action onExit)
    {
        EnterBtn.onClick.AddListener(() => { onEnter(); OnUIClickSound(); });
        ExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
        AreaOutBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
    }

    public void UpdateDropExpText(string str)
    {
        DropExp.text = $"획득 가능한 경험치 : {str}";
    }

    public void UpdateAppearMonsterAndLevelCount(int monsterCount, int lowLevel, int maxLevel)
    {
        ApearMonsterAndLevel.text = $"몬스터 수 : {monsterCount}\n" +
                                    $"권장 레벨 : {lowLevel} ~ {maxLevel}";
    }

    public void UpdateFatigueCostText(int cost)
    {
        FatigueCost.text = $"피로도 - {cost}";
    }

    public void ApearMonsterImage(Sprite sprite)
    {
        var imageObject = new GameObject("MonsterImage");
        var image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.transform.SetParent(ApearMonsterContainer);
    }

    public void DropItemImage(Sprite sprite)
    {
        var imageObject = new GameObject("DropItemImage");
        var image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.transform.SetParent(DropItemContainer);
    }

    public void DropEggImage(Sprite sprite)
    {
        var imageObject = new GameObject("DropEggImage");
        var image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.transform.SetParent(DropEggContainer);
    }

}

public class UIPopup_DungeonEnterCheck_Presenter
{
    private UIPopup_DungeonEnterCheck _view;
    private Dictionary<string, DungeonInfoData> _dungeonInfoDic;
    private string _dungeonId;

    public UIPopup_DungeonEnterCheck_Presenter(UIPopup_DungeonEnterCheck view)
    {
        _view = view;
    }

    public void Init()
    {
        SetButton();
        UpdateUI();
        Initialize();
    }

    private void SetButton()
    {
        _view.SetButton(OnEnter, OnExit);
    }

    private void OnEnter()
    {
        // 계속 클라에서 켜두는 경우 서버에 피로도 업데이트 필요
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(0);

        int needFatigue = DataManager.Instance.ReadOnlyDataSystem.DungeonInfo[_view.DungeonId].Fatigue;

        // 피로도 부족 
        if (GameManager.Instance.FatigueSystem.FatigueData.CurrentFatigue < needFatigue)
        {
            UIPopup_Warning warning = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            warning.SetText("피로도가 부족합니다.");
        }
        // 피로도 충분 : 나갈 때 피로도 깎기
        else
        {
            SceneManager.LoadScene(Define.DungeonScene);
        }
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
    }

    private void UpdateUI()
    {
        var dungeonManager = GameManager.Instance.DungeonStageSystem;
        if (dungeonManager == null) return;

        if (dungeonManager.DungeonInfo.TryGetValue(dungeonManager.SelectedStageId, out var dungeonInfo))
        {
            _view.UpdateDropExpText($"{dungeonInfo.ClearExp}");
            _view.UpdateAppearMonsterAndLevelCount(dungeonInfo.MonsterSpawnNum, dungeonInfo.MinLevel, dungeonInfo.MaxLevel);
            _view.UpdateFatigueCostText(dungeonInfo.Fatigue);

            if (dungeonManager.DungeonMonsterProbability.TryGetValue(dungeonManager.SelectedStageId, out var monsters))
            {
                foreach (var monster in monsters)
                {
                    var sprite = GetMonsterSprite(monster.MonsterId); 
                    _view.ApearMonsterImage(sprite);
                }
            }

            if (dungeonManager.DungeonEggProbability.TryGetValue(dungeonManager.SelectedStageId, out var eggs))
            {
                foreach (var egg in eggs)
                {
                    var sprite = GetEggSprite(egg.EggId); 
                    _view.DropEggImage(sprite);
                }
            }

            foreach (var monster in monsters)
            {
                if (DataManager.Instance.ReadOnlyDataSystem.MonsterDropItem.TryGetValue(monster.MonsterId, out var items))
                {
                    foreach (var item in items)
                    {
                        var sprite = GetItemSprite(item.ItemId); 
                        _view.DropItemImage(sprite);
                    }
                }
            }
        }
    }

    private Sprite GetMonsterSprite(string monsterId)
    {
        return ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", monsterId);
    }

    private Sprite GetEggSprite(string eggId)
    {
        return ResourceManager.Instance.LoadSprite($"Icon/EggIcon/eggs", eggId);
    }
    
    private Sprite GetItemSprite(string itemId)
    {
        return ResourceManager.Instance.Load<Sprite>($"Icon/ItemIcon/{itemId}");
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
    }

    public void Initialize()
    {
        _dungeonId = _view.DungeonId;
        _view.DungeonTitle.text = $"{FormatStageText(DataManager.Instance.ReadOnlyDataSystem.DungeonInfo[_view.DungeonId].Stage)} Stage ";
    }

    private string FormatStageText(int stage)
    {
        int major = stage / 10;
        int minor = stage % 10;
        return $"{major}-{minor}";
    }
}
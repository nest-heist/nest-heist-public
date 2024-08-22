using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UIPopup_MonsterInventory : UIPopup
{
    [Header("Text")]
    public TextMeshProUGUI MonsterNameText;
    public TextMeshProUGUI MonsterTypeText;
    public TextMeshProUGUI MonsterRankText;
    public TextMeshProUGUI MonsterLevelText;

    public TextMeshProUGUI MonsterSkillNameText;
    public TextMeshProUGUI MonsterSkillDescriptionText;

    public TextMeshProUGUI MonsterHPText;
    public TextMeshProUGUI MonsterATKText;
    public TextMeshProUGUI MonsterDefenceText;
    public TextMeshProUGUI MonsterCriticalText;
    public TextMeshProUGUI MonsterCriticalDamageText;

    [Header("Image")]
    public Image MonsterImage;

    [Header("Button")]
    public Button OnExitBtn;
    public Button GrowUpBtn;

    [Header("Script")]
    public UISubItem_MonsterButton MonsterButton;

    [Header("Transform")]
    public Transform Right;

    public UIScene_MonsterInventory_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIScene_MonsterInventory_Presenter(this);
        Presenter.Init();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void UpdateMonsterInfo(UserMonster userMonster)
    {
        MonsterNameText.text = $"{DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[userMonster.UserData.MonsterId].Name}";

        var attributeTypes = DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[userMonster.UserData.MonsterId].AttributeTypeList;
        var attributeNames = attributeTypes.Select(attr => Presenter.AttributeTypeToKorean[attr]);

        MonsterTypeText.text = $"{string.Join(", ", attributeNames)}";
        MonsterSkillNameText.text = $"{DataManager.Instance.ReadOnlyDataSystem.MonsterSkillInfo[userMonster.UserData.SkillId].Name}";
        MonsterSkillDescriptionText.text = $"{DataManager.Instance.ReadOnlyDataSystem.MonsterSkillInfo[userMonster.UserData.SkillId].Desc}";

        MonsterLevelText.text = $"{userMonster.UserData.Level}";
        MonsterRankText.text = $"{new string('☆', userMonster.UserData.Rank)}"; // Rank 값을 ☆ 문자로 변환하여 Rank: 와 함께 표시

        // 몬스터 스탯 설정
        MonsterHPText.text = $"{userMonster.CurrentStat.MaxHP}";
        MonsterATKText.text = $"{userMonster.CurrentStat.Attack}";
        MonsterDefenceText.text = $"{userMonster.CurrentStat.Defence}";
        MonsterCriticalText.text = $"{(int)userMonster.CurrentStat.Critical}";
        MonsterCriticalDamageText.text = $"{(int)userMonster.CurrentStat.CriticalDamage}";

        // 몬스터 이미지 설정
        string imagePath = "Icon/MonsterIcon/monster-icon";
        MonsterImage.sprite = ResourceManager.Instance.LoadSprite(imagePath, userMonster.UserData.MonsterId);

        if (MonsterImage.sprite == null)
        {
            Logging.LogError($"이미지를 불러올 수 없습니다: {imagePath}");
        }
    }

    public void SetButton(Action onExit, Action onGrowUp)
    {
        OnExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
        // PartnerMonsterBtn.onClick.AddListener(() => onPartnerResister());
        GrowUpBtn.onClick.AddListener(() => { onGrowUp(); OnUIClickSound(); });
    }

    public UISubItem_MonsterButton CreateMonsterButton()
    {
        return Instantiate(MonsterButton, Right);
    }
}

public class UIScene_MonsterInventory_Presenter
{
    private UIPopup_MonsterInventory _view;
    public UserMonster CurrentMonster;

    public Dictionary<AttributeType, string> AttributeTypeToKorean = new Dictionary<AttributeType, string>
    {
        { AttributeType.Water, "물" },
        { AttributeType.Fire, "불" },
        { AttributeType.Grass, "풀" },
        { AttributeType.Ground, "땅" },
        { AttributeType.Electric, "전기" },
        { AttributeType.Ice, "얼음" },
        { AttributeType.Dragon, "용" },
        { AttributeType.Dark, "어둠" },
        { AttributeType.Normal, "일반" }
    };

    public UIScene_MonsterInventory_Presenter(UIPopup_MonsterInventory view)
    {
        _view = view;
    }

    public void Init()
    {
        Initialize();
        SetButton();
    }

    public void Initialize()
    {
        RefreshMonsterButtons();

        // 첫 번째 몬스터를 선택하여 정보를 표시
        List<UserMonster> sortedMonsters = GameManager.Instance.UserMonsterSystem.GetSortedMonsters();
        if (sortedMonsters.Count > 0)
        {
            UserMonster firstMonster = sortedMonsters[0];
            OnMonsterButtonClick(firstMonster.UserData.Id);
        }
    }

    public void RefreshMonsterButtons()
    {
        ClearButtons();

        // UserMonsterManager에서 정렬된 몬스터 목록 가져오기
        List<UserMonster> sortedMonsters = GameManager.Instance.UserMonsterSystem.GetSortedMonsters();

        foreach (var userMonster in sortedMonsters)
        {
            AddMonsterButton(userMonster, () => OnMonsterButtonClick(userMonster.UserData.Id));
        }
    }

    public void OnMonsterButtonClick(string userMonsterId)
    {
        if (GameManager.Instance.UserMonsterSystem.UserMonsterList.TryGetValue(userMonsterId, out UserMonster userMonster))
        {
            _view.UpdateMonsterInfo(userMonster);
            CurrentMonster = userMonster;
        }
        else
        {
            Logging.LogError($"Monster ID: {userMonsterId} 없음.");
        }
    }

    private void SetButton()
    {
        _view.SetButton(OnExit, OnGrowUp);
    }

    public void ClearButtons()
    {
        foreach (Transform child in _view.Right.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddMonsterButton(UserMonster userMonster, Action onClickAction)
    {
        _view.CreateMonsterButton().Initialize(userMonster, onClickAction);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }

    private void OnGrowUp()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_GrowUpScreen>();
    }
}

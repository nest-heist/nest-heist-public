using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScene_LobbyScene : UIScene
{
    [Header("Button")]
    public Button GotoBossButton;
    public Button GotoDungeonButton;
    public Button GotoHatchingButton;
    public Button GotoPlayerStatButton;
    public Button GotoInventoryButton;
    public Button GotoMonsterInventoryButton;
    public Button GotoPartySetScreenButton;
    public Button GotoQuestButton;
    public Button SettingButton;
    public Button GuidButton;

    private UISubItem_UserCurrency _middleUpCurrency;
    [SerializeField] private List<UISubItem_LobbyLiveMonster> _liveMonsterList;

    [Header("Text")]
    public TextMeshProUGUI UserLevelText;
    public TextMeshProUGUI UserNameText;

    [Header("Image")]
    public Image PlayerImage;
    public Image PlayerEXPImage;

    private UIScene_LobbyScene_Presenter _presenter;

    [Header("Tween")]
    [SerializeField] private List<DOTweenAnimation> _tweens;

    /// <summary>
    /// 버튼 연결 말고는 다 Refresh 해주기
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIScene_LobbyScene_Presenter(this);
        _presenter.Init();

        _middleUpCurrency = GetComponentInChildren<UISubItem_UserCurrency>();
        _middleUpCurrency.Init();

        DataManager.Instance.ServerDataSystem.OnUpdateParty += _presenter.LiveMonsters;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.Instance.Invoke(EventType.OnLogin, this);
        AudioManager.Instance.PlayBGM("IntroLobbyBGM");

        CheckTutorial();
    }

    /// <summary>
    /// 튜토리얼 띄워줄지말지 체크
    /// </summary>
    public void CheckTutorial()
    {
        if (DataManager.Instance.LocalDataSystem.ClickTutorial.CurrentTutorialIndex < 8)
        {
            UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial_Party_1>();
        }
    }

    private void OnDestroy()
    {
        DataManager.Instance.ServerDataSystem.OnUpdateParty -= _presenter.LiveMonsters;
    }

    /// <summary>
    /// 로비는 다시 켜질 때 Popup처럼 삭제됬다가 다시 생성되는 것이 아니기 때문에 OnEnable에서 초기화를 해준다.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        OnEnableDotween();

        _presenter.Init();

        if (_middleUpCurrency == null)
        {
            _middleUpCurrency = GetComponentInChildren<UISubItem_UserCurrency>();
        }
        _middleUpCurrency.Init();
        FirstConnect();
    }

    public void FirstConnect()
    {
        Tutorial generalTutorial = GameManager.Instance.PageTutorialSystem.GetTutorialById("general_tutorial");
        if (generalTutorial != null)
        {
            GameManager.Instance.PageTutorialSystem.ShowGeneralTutorialIfNotShown(generalTutorial);
        }
    }

    private void OnEnableDotween()
    {
        foreach (DOTweenAnimation tween in _tweens)
        {
            tween.DORestart();
        }
    }

    public void SetButton(Action OnGotoPlayerStat, Action OnGotoInventory, Action OnGotoMonsterInventory,
        Action OnGotoHatching, Action OnGotoDungeon, Action OnGotoPartySetScreen, Action OnGotoQuest,
        Action OnSetting, Action OnGuide)
    {
        GotoPlayerStatButton.onClick.AddListener(() => { OnGotoPlayerStat(); OnUIClickSound(); });
        GotoInventoryButton.onClick.AddListener(() => { OnGotoInventory(); OnUIClickSound(); });
        GotoMonsterInventoryButton.onClick.AddListener(() => { OnGotoMonsterInventory(); OnUIClickSound(); });
        GotoHatchingButton.onClick.AddListener(() => { OnGotoHatching(); OnUIClickSound(); });
        GotoDungeonButton.onClick.AddListener(() => { OnGotoDungeon(); OnUIClickSound(); });
        GotoPartySetScreenButton.onClick.AddListener(() => { OnGotoPartySetScreen(); OnUIClickSound(); });
        GotoQuestButton.onClick.AddListener(() => { OnGotoQuest(); OnUIClickSound(); });
        SettingButton.onClick.AddListener(() => { OnSetting(); OnUIClickSound(); });
        GuidButton.onClick.AddListener(() => { OnGuide(); OnUIClickSound(); });
    }

    public void SetText(string level, string username)
    {
        UserLevelText.text = level;
        UserNameText.text = username;
    }

    public void SetImage(Sprite playerImage)
    {
        PlayerImage.sprite = GameManager.Instance.UserInfoSystem.UserProfileSprite;
    }

    public void SetMiddlePartnerMonsters(List<string> monsterIds)
    {
        for (int i = 0; i < _liveMonsterList.Count; i++)
        {
            if (i < monsterIds.Count)
            {
                _liveMonsterList[i].Initialize(monsterIds[i]);
            }
            else
            {
                _liveMonsterList[i].Clear();
            }

        }
    }

    public void SetBar(float fill)
    {
        PlayerEXPImage.fillAmount = fill;
    }
}

public class UIScene_LobbyScene_Presenter : Presenter
{
    private UIScene_LobbyScene _view;

    public UIScene_LobbyScene_Presenter(UIScene_LobbyScene view)
    {
        _view = view;

        SetButton();

        // 유저 정보 시스템에서 이미지 받아오면 적용 : 파이어베이스 로그인 시 유저 이미지 다운받아서 오래 걸린다
        GameManager.Instance.UserInfoSystem.OnUserProfileSpriteLoaded -= SetSprite;
        GameManager.Instance.UserInfoSystem.OnUserProfileSpriteLoaded += SetSprite;
    }

    public void Init()
    {
        SetText();
        SetSprite();
        LiveMonsters();
        SetBar();
    }

    private void SetButton()
    {
        _view.SetButton(UserStat, Inventory, MonsterInventory, Hatching, Dungeon, PartySetScreen, Quest, Setting, GuidOpen);
        _view.GotoBossButton.onClick.AddListener(() =>
        {
            if (DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds.Length > 0)
            {
                UIManager.Instance.InstanciateUIPopup<UIPopup_BossSelect>();
                _view.gameObject.SetActive(false);
            }
            else
            {
                UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
                popup.SetText("파티에 편성된 몬스터가 없습니다.");
            }
        });
    }

    private void SetText()
    {
        string userLevel = DataManager.Instance.ServerDataSystem.User.Level.ToString();
        string username = GameManager.Instance.UserInfoSystem.UserProfileName;

        _view.SetText(userLevel, username);
    }

    /// <summary>
    /// 플레이어 이미지 - 파이어베이스 로그인 시 받아온 이미지
    /// </summary>
    private void SetSprite()
    {
        Sprite playerSprite = GameManager.Instance.UserInfoSystem.UserProfileSprite;

        _view.SetImage(playerSprite);
    }

    public void LiveMonsters(ServerUserGameInfoData body = null)
    {
        List<string> monsterIds = new List<string>();
        foreach (string id in DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds)
        {
            monsterIds.Add(GameManager.Instance.UserMonsterSystem.UserMonsterList[id].UserData.MonsterId);
        }
        _view.SetMiddlePartnerMonsters(monsterIds);
    }

    /// <summary>
    /// 플레이어 경험치 바
    /// </summary>
    private void SetBar()
    {
        int expAmount = DataManager.Instance.ServerDataSystem.User.Exp;
        int expLevel = DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[DataManager.Instance.ServerDataSystem.User.Level].SumOfExp;

        float fillAmount = (float)expAmount / expLevel;
        _view.SetBar(fillAmount);
    }

    private void UserStat()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_UserStat>();
    }

    private void Inventory()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Inventory>();
        _view.gameObject.SetActive(false);
    }

    private void MonsterInventory()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_MonsterInventory>();
        _view.gameObject.SetActive(false);
    }

    private void Hatching()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Incubator>();
        _view.gameObject.SetActive(false);
    }

    private void Dungeon()
    {
        if (DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds.Length > 0)
        {
            UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonTypeSelect>();
            _view.gameObject.SetActive(false);
        }
        else
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("파티에 편성된 몬스터가 없습니다.");
        }
    }

    private void PartySetScreen()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_ShowPartyStateScreen>();
        _view.gameObject.SetActive(false);
    }

    private void Quest()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_QuestList>();
        _view.gameObject.SetActive(false);
    }

    private void Setting()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Setting>();
    }

    private void GuidOpen()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial>();
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_UserStat : UIPopup
{
    [Header("Button")]
    public Button GotoLobbyButton;

    [Header("Text")]
    public TextMeshProUGUI UserNameText;
    public TextMeshProUGUI UserLevelText;
    public TextMeshProUGUI UserHealthText;
    public TextMeshProUGUI UserDefenceText;
    public TextMeshProUGUI UserWalkSpeedText;

    [Header("Image")]
    public Image UserImage;

    [Header("GameObject")]
    public GameObject RightContent;

    private UIPopup_UserStat_Presenter _presenter;

    protected override void Awake()
    {
        base.Awake();
        _presenter = new UIPopup_UserStat_Presenter(this);
        _presenter.Init();
    }

    public void SetButton(Action OnGotoLobby)
    {
        GotoLobbyButton.onClick.AddListener(() => { OnGotoLobby(); OnUIClickSound(); });
    }

    public void SetText(string userName, string userLevel, string userHealth, string userDefence, string userWalkSpeed)
    {
        UserNameText.text = userName;
        UserLevelText.text = $"Lv {userLevel}";
        UserHealthText.text = userHealth;
        UserDefenceText.text = userDefence;
        UserWalkSpeedText.text = userWalkSpeed;
    }

    public void SetSprite(Sprite userImage)
    {
        UserImage.sprite = userImage;
    }
}

public class UIPopup_UserStat_Presenter : Presenter
{
    private UIPopup_UserStat _view;
    private List<UserMonster> _partnerMonsters => GameManager.Instance.UserMonsterSystem.PartyUserMonsterList;
    

    private PlayerAllStatData _playerAllSatData;
    public PlayerStat CurrentStat { get; private set; }
    public PlayerStat BaseStat { get; private set; }
    public PlayerStat LevelStat { get; private set; }

    public UIPopup_UserStat_Presenter(UIPopup_UserStat view)
    {
        _view = view;

        SetButton();
    }

    public void Init()
    {
        SetCurrentUserStat();

        SetText();
        SetSprite();

        MakeSubItem_UserStatMonster();
    }

    private void MakeSubItem_UserStatMonster()
    {
        foreach (Transform child in _view.RightContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (UserMonster partnerMonster in _partnerMonsters)
        {
            UISubItem_UserStatMonster item = UIManager.Instance.InstanciateSubItem<UISubItem_UserStatMonster>(_view.RightContent.transform);
            item.Init(partnerMonster);
        }
    }

    public void SetCurrentUserStat()
    {
        CurrentStat = new PlayerStat();
        BaseStat = new PlayerStat();
        LevelStat = new PlayerStat();

        _playerAllSatData = DataManager.Instance.ReadOnlyDataSystem.PlayerAllStat[DataManager.Instance.ServerDataSystem.User.PlayerId];
        // 기본 스텟 초기화
        BaseStat = _playerAllSatData.BaseStat.ReturnStat().DeepCopy();
        CurrentStat = BaseStat.DeepCopy();

        // 초기 스텟 계산
        UpdateLevelStat();
    }

    public void UpdateLevelStat()
    {
        LevelStat = _playerAllSatData.LevelStat.ReturnStat().StatCalculator(StatCalculator.CalculateLevelStat, DataManager.Instance.ServerDataSystem.User.Level);
        CurrentStat.Add(LevelStat);
    }

    private void SetSprite()
    {
        Sprite playerSprite = GameManager.Instance.UserInfoSystem.UserProfileSprite;

        _view.SetSprite(playerSprite);
    }

    private void SetText()
    {
        string userName = GameManager.Instance.UserInfoSystem.UserProfileName;
        string userlevel = DataManager.Instance.ServerDataSystem.User.Level.ToString();

        string userHealth = ((int)CurrentStat.MaxHP).ToString();
        string userDefence = ((int)CurrentStat.Avoid).ToString();
        string userWalkSpeed = ((int)CurrentStat.WalkSpeed).ToString();

        _view.SetText(userName, userlevel, userHealth, userDefence, userWalkSpeed);
    }

    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    public void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(false);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }
}

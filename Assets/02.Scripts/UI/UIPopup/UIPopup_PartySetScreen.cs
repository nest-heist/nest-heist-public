using System;
using System.Collections.Generic;
using static Define;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_PartySetScreen : UIPopup
{
    [Header("Button")]
    public Button CloseButton;
    public Button UpdatePartyButton;

    [Header("Script")]
    public UISubItem_MonsterButton MonsterButton;

    [Header("Transform")]
    public Transform Right;

    public UIPopup_PartyScreen_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIPopup_PartyScreen_Presenter(this);
        UpdatePartyButton.onClick.AddListener(() => { Presenter.OnUpdateParty(); OnUIClickSound(); });
        CloseButton.onClick.AddListener(OnClosePopup);
        CloseButton.onClick.AddListener(OnUIClickSound);
        Presenter.Init();
    }


    // Panel의 Dotween Animation의 이벤트에서 사용
    public void OnClosePopup()
    {
        UIManager.Instance.DestoryUIPopup(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public UISubItem_MonsterButton CreateMonsterButton()
    {
        return Instantiate(MonsterButton, Right);
    }
}

public class UIPopup_PartyScreen_Presenter
{
    private UIPopup_PartySetScreen _view;
    public SortedDictionary<string, UserMonster> TempPartyList;
    public SortedDictionary<string, UISubItem_MonsterButton> PartyMonsterButton;
    private List<string> TempPartyListOrder;

    public UIPopup_PartyScreen_Presenter(UIPopup_PartySetScreen view)
    {
        _view = view;
    }

    public void Init()
    {
        TempPartyListOrder = new List<string>();
        TempPartyList = new SortedDictionary<string, UserMonster>();
        PartyMonsterButton = new SortedDictionary<string, UISubItem_MonsterButton>();
        foreach (var monster in GameManager.Instance.UserMonsterSystem.PartyUserMonsterList)
        {
            TempPartyList.Add(monster.UserData.Id, monster);
            TempPartyListOrder.Add(monster.UserData.Id);
        }
        RefreshMonsterButtons();
    }
    public void RefreshMonsterButtons()
    {
        ClearButtons();

        // UserMonsterManager에서 정렬된 몬스터 목록 가져오기
        List<UserMonster> sortedMonsters = GameManager.Instance.UserMonsterSystem.GetSortedMonsters();

        foreach (var userMonster in GameManager.Instance.UserMonsterSystem.UserMonsterList)
        {
            var monsterButton = _view.CreateMonsterButton();
            bool isInParty = TempPartyList.ContainsKey(userMonster.Key);
            monsterButton.Initialize(userMonster.Value, () => OnMonsterButtonClick(monsterButton, userMonster.Key));
            if (isInParty)
            {
                PartyMonsterButton.Add(userMonster.Key, monsterButton);
            }
        }
        SetShowPartyMark();
    }
    public void ClearButtons()
    {
        foreach (Transform child in _view.Right.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void OnMonsterButtonClick(UISubItem_MonsterButton monsterButton, string userMonsterId)
    {
        if (GameManager.Instance.UserMonsterSystem.UserMonsterList.TryGetValue(userMonsterId, out UserMonster userMonster))
        {
            if (TempPartyList.ContainsKey(userMonsterId))
            {
                TempPartyListOrder.Remove(userMonsterId);
                TempPartyList.Remove(userMonsterId);
                PartyMonsterButton.Remove(userMonsterId);
                monsterButton.SetIndex(-1);
            }
            else if (TempPartyList.Count < 3)
            {
                TempPartyListOrder.Add(userMonsterId);
                TempPartyList.Add(userMonsterId, userMonster);
                PartyMonsterButton.Add(userMonsterId, monsterButton);
            }
            SetShowPartyMark();
        }
    }

    public void SetShowPartyMark()
    {
        int i = 0;
        foreach (var id in TempPartyListOrder)
        {
            PartyMonsterButton[id].SetIndex(i++);
        }
    }

    public void OnUpdateParty()
    {
        // 파티 리스트가 비어 있음 서버 업뎃 불가
        if (TempPartyListOrder.Count <= 0)
        {
            return;
        }

        UpdatePartyOnServer();
    }

    private async void UpdatePartyOnServer()
    {
        await LoadingManager.Instance.RunWithLoading(async () =>
        {
            await DataManager.Instance.ServerDataSystem.UpdateParty(CreateUpdatePartyBody());
        }, 
        onLoaded: () =>
        {
            UIManager.Instance.DestoryUIPopup(_view);
        },
        onError: (message) =>
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"파티갱신 실패: {message}", "UpdatePartyOnServer", sceneName);
        });
    }

    private UpdatePartyBody CreateUpdatePartyBody()
    {
        return new UpdatePartyBody
        {
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            MonsterIds = TempPartyListOrder.ToArray()
        };
    }
}
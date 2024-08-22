using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_ShowPartyStateScreen : UIPopup
{
    [Header("Button")]
    public Button OnExitBtn;
    public Button[] OnSetScreenBtn;
    [SerializeField] private List<UISubItem_LobbyLiveMonster> _liveMonsterList;

    public UIPopup_ShowPartyStateScreen_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();

        Presenter = new UIPopup_ShowPartyStateScreen_Presenter(this);

        DataManager.Instance.ServerDataSystem.OnUpdateParty += Presenter.SetLiveMonsters;
        Presenter.Init();
    }

    private void OnDestroy()
    {
        DataManager.Instance.ServerDataSystem.OnUpdateParty -= Presenter.SetLiveMonsters;
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

    public void SetButton(Action onExit, Action onSetScreen)
    {
        OnExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });

        foreach (Button btn in OnSetScreenBtn)
        {
            btn.onClick.AddListener(() => { onSetScreen();});
        }
    }
}

public class UIPopup_ShowPartyStateScreen_Presenter
{
    private UIPopup_ShowPartyStateScreen _view;
    private UIPopup_PartySetScreen _setScreen;

    public void Init()
    {
        SetButton();
        SetLiveMonsters();
    }

    public UIPopup_ShowPartyStateScreen_Presenter(UIPopup_ShowPartyStateScreen view)
    {
        _view = view;
    }

    private void SetButton()
    {
        _view.SetButton(OnExit, OnSetScreen);
    }


    private void OnExit()
    {
        UIManager.Instance.UIScene.gameObject.SetActive(true);
        UIManager.Instance.DestoryAllUIPopup();
    }

    public void SetLiveMonsters(ServerUserGameInfoData body = null)
    {
        List<string> monsterIds = new List<string>();
        foreach (var id in DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds)
        {
            monsterIds.Add(GameManager.Instance.UserMonsterSystem.UserMonsterList[id].UserData.MonsterId);
        }
        _view.SetMiddlePartnerMonsters(monsterIds);
    }

    private void OnSetScreen()
    {
        if (_setScreen == null)
        {
            UIPopup_PartySetScreen setScreen = UIManager.Instance.InstanciateUIPopup<UIPopup_PartySetScreen>();
            _setScreen = setScreen;
        }
        else
        {
            UIManager.Instance.DestoryUIPopup(_setScreen);
            _setScreen = null;
            UIPopup_PartySetScreen setScreen = UIManager.Instance.InstanciateUIPopup<UIPopup_PartySetScreen>();
            _setScreen = setScreen;
        }
    }
}
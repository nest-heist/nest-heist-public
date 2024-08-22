using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Incubator : UIPopup
{
    [Header("Button")]
    public Button GotoLobbyButton;
    public Button GuidButton;
    public GameObject WinnerList;

    private UIPopup_Incubator_Presenter _presenter;

    private UISubItem_Incubator _incubator;

    private UISubItem_UserCurrency _middleUpCurrency;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIPopup_Incubator_Presenter(this);
        DataManager.Instance.ServerDataSystem.OnGetWinners += SetWinnerSubItem;
    }

    /// <summary>
    /// 알인벤토리 갔다가 다시 켜주기 때문에 OnEnable에서 초기화
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        SetGuidButtonActive();
        SetWinnerSubItem();

        if (_middleUpCurrency == null)
        {
            _middleUpCurrency = GetComponentInChildren<UISubItem_UserCurrency>();
        }
        _middleUpCurrency.Init();
    }

    void OnDestroy()
    {
        DataManager.Instance.ServerDataSystem.OnGetWinners -= SetWinnerSubItem;
    }

    protected override void Start()
    {
        base.Start();
        FirstConnectIncubator();
    }

    void FirstConnectIncubator()
    {
        Tutorial eggIncubatorTutorial = GameManager.Instance.PageTutorialSystem.GetTutorialById("egg_incubator");
        if (eggIncubatorTutorial != null)
        {
            GameManager.Instance.PageTutorialSystem.ShowEggIncubatorTutorialIfNotShown(eggIncubatorTutorial);
        }
    }

    public void SetButton(Action OnGoToLobby)
    {
        GotoLobbyButton.onClick.AddListener(() => { OnGoToLobby(); OnUIClickSound(); });
        GuidButton.onClick.AddListener(() =>
        {
            UIManager.Instance.InstanciateUIPopup<UIPopup_EggIncubatorTutorial>();
            OnUIClickSound();
        });
    }

    private void SetGuidButtonActive()
    {
        bool isGeneralTutorialEnabled = GameManager.Instance.PageTutorialSystem.IsTutorialEnabled("egg_incubator");
        GuidButton.gameObject.SetActive(isGeneralTutorialEnabled);
    }

    private void SetWinnerSubItem()
    {
        List<ServerWinnerData> winnerList = DataManager.Instance.ServerDataSystem.WinnerList;

        foreach (Transform child in WinnerList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (winnerList.Count > 0)
        {
            foreach (ServerWinnerData data in winnerList)
            {
                UISubItem_CouponWinner subItem = UIManager.Instance.InstanciateSubItem<UISubItem_CouponWinner>(WinnerList.transform);

                subItem.SetEmail(Define.MaskEmail(data.Email));
            }
        }
        else
        {
            UISubItem_CouponWinner subItem = UIManager.Instance.InstanciateSubItem<UISubItem_CouponWinner>(WinnerList.transform);
            subItem.SetEmail("당첨자가 없습니다.");
        }
    }

}

public class UIPopup_Incubator_Presenter
{
    private UIPopup_Incubator _view;

    public UIPopup_Incubator_Presenter(UIPopup_Incubator view)
    {
        _view = view;

        SetButton();
    }

    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    public void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_QuestReward : UIPopup
{
    [Header("Button")]
    public Button ExitButton;

    [Header("GameObject")]
    public GameObject LeftRewardList;

    public UIPopup_QuestReward_Presenter Presenter;

    public void Init(List<Reward> rewardList)
    {
        Presenter = new UIPopup_QuestReward_Presenter(this, rewardList);
    }

    public void SetButton(Action OnExit)
    {
        ExitButton.onClick.AddListener(() => { OnExit(); OnUIClickSound(); });
    }
}

public class UIPopup_QuestReward_Presenter : Presenter
{
    private UIPopup_QuestReward _view;
    private List<Reward> _rewardList;

    public UIPopup_QuestReward_Presenter(UIPopup_QuestReward view, List<Reward> rewardList)
    {
        _view = view;
        _rewardList = rewardList;

        SetButton();
        SetRewardList();
    }

    private void SetRewardList()
    {
        foreach (Reward reward in _rewardList)
        {
            UISubItem_QuestRewardItem rewardsubitem = UIManager.Instance.InstanciateSubItem<UISubItem_QuestRewardItem>(_view.LeftRewardList.transform);
            rewardsubitem.Presenter.SetReward(reward);
        }
    }

    private void SetButton()
    {
        _view.SetButton(() =>
        {
            UIManager.Instance.DestoryUIPopup(this._view);
        });
    }

    
}
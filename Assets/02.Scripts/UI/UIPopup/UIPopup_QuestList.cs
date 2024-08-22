using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_QuestList : UIPopup
{
    [Header("Button")]
    public Button ExitButton;

    public Button QuestTestButton;

    [Header("Text")]
    public TextMeshProUGUI QuestTitleText;
    public TextMeshProUGUI TaskDescText;
    public TextMeshProUGUI QuestDescText;

    [Header("GameObject")]
    public GameObject LeftRewardList;
    public GameObject RightQuestList;

    public UIPopup_QuestList_Presenter Presenter;

    protected override void Start()
    {
        base.Start();

        Presenter = new UIPopup_QuestList_Presenter(this);
        Presenter.Init();
    }

    public void SetButton(Action OnExit)
    {
        ExitButton.onClick.AddListener(() => { OnExit(); OnUIClickSound(); });
    }

    public void SetText(string questTitleText, string taskDescText, string questDescText)
    {
        QuestTitleText.text = questTitleText;
        TaskDescText.text = taskDescText;
        QuestDescText.text = questDescText;
    }
}

public class UIPopup_QuestList_Presenter : Presenter
{
    private UIPopup_QuestList _view;

    QuestSystem _questSystem;
    private Quest _seletedQuest;

    public UIPopup_QuestList_Presenter(UIPopup_QuestList view)
    {
        _view = view;

        _questSystem = GameManager.Instance.QuestSystem;
        _questSystem.OnQuestRegistered += RegisterQuest;

        SetButton();
    }

    private void SetButton()
    {
        _view.SetButton(Exit);
    }

    public void Init()
    {
        RefreshRightQuestList();

        SelectFirstQuest();
    }

    // ----------- 오른쪽 퀘스트목록 -------------

    private void RefreshRightQuestList()
    {
        DestoryRightQuestListSubtems();

        foreach (Quest quest in _questSystem.ActiveQuests)
        {
            UISubItem_Quest questsubitem = UIManager.Instance.InstanciateSubItem<UISubItem_Quest>(_view.RightQuestList.transform);
            questsubitem.Init();

            questsubitem.Presenter.SetQuest(quest, this._view);
        }
    }

    private void DestoryRightQuestListSubtems()
    {
        foreach (Transform child in _view.RightQuestList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void RegisterQuest(Quest quest)
    {
        UISubItem_Quest questsubitem = UIManager.Instance.InstanciateSubItem<UISubItem_Quest>(_view.RightQuestList.transform);
        questsubitem.Init();

        questsubitem.Presenter.SetQuest(quest, this._view);
    }

    public void SelectFirstQuest()
    {
        // 퀘스트 없을 떄 
        if (_questSystem.ActiveQuests.Count == 0)
        {
            _view.SetText("퀘스트 끝!", "현재 남아있는 할 일이 없다!", "모든 퀘스트를 완료했습니다!");
            DestoryLeftRewardListSubtems();
            return;
        }

        SelectQuest(_questSystem.ActiveQuests[0]);
    }

    public void SelectQuest(Quest quest)
    {
        if (quest == null)
        {
            Logging.LogError("UIPopup_QuestList::SelectQuest() 퀘스트가 널이다");
            return;
        }

        _seletedQuest = quest;
        _seletedQuest.OnSubtaskSuccessChanged -= ChangeSubTaskSuccess;
        _seletedQuest.OnSubtaskSuccessChanged += ChangeSubTaskSuccess;

        SetText();
        SetRewardList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="subtask"></param>
    /// <param name="currectSuccess"></param>
    /// <param name="prevSuccess"></param>
    private void ChangeSubTaskSuccess(Quest quest, Subtask subtask, int currectSuccess, int prevSuccess)
    {
        SetText();
    }

    // ----------- 왼쪽 퀘스트 상세 -------------

    private void DestoryLeftRewardListSubtems()
    {
        foreach (Transform child in _view.LeftRewardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void SetText()
    {
        QuestInfoData questinfo = _seletedQuest.QuestInfoData;

        string questTitleText = questinfo.Name;

        // 여기는 문자열 + 필요하다 
        string taskDescText = "";
        foreach (Subtask task in _seletedQuest.CurrentSubtaskGroup.SubtaskList)
        {
            taskDescText += BuildText(task) + "\n";
        }

        string questDescText = questinfo.Desc;

        _view.SetText(questTitleText, taskDescText, questDescText);
    }

    private string BuildText(Subtask task)
    {
        return $"{task.TaskInfoData.Desc} [{task.CurrentSuccess}/{task.TaskInfoData.NeedSuccessToCompleted}]";
    }

    private void SetRewardList()
    {
        List<Reward> rewards = _seletedQuest.RewardList;

        DestoryLeftRewardListSubtems();

        foreach (Reward reward in rewards)
        {
            UISubItem_QuestRewardItem rewardsubitem = UIManager.Instance.InstanciateSubItem<UISubItem_QuestRewardItem>(_view.LeftRewardList.transform);
            rewardsubitem.Presenter.SetReward(reward);
        }
    }

    private void Exit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISubItem_Quest : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI QuestText;

    [Header("Button")]
    public Button QuestButton;
    public Button CompleteButton;

    public UISubItem_Quest_Presenter Presenter;

    protected override void Start()
    {
        base.Start();

        if (Presenter == null)
        {
            Presenter = new UISubItem_Quest_Presenter(this);
        }
    }

    public void Init()
    {
        if (Presenter == null)
        {
            Presenter = new UISubItem_Quest_Presenter(this);
        }
    }

    public void SetText(string questText)
    {
        QuestText.text = questText;
    }

    public void SetButton(Action OnClickQuestButton, Action OnClickCompleteButton)
    {
        QuestButton.onClick.AddListener(() => { OnClickQuestButton(); OnUIClickSound(); });
        CompleteButton.onClick.AddListener(() => { OnClickCompleteButton(); OnUIClickSound(); });
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}

public class UISubItem_Quest_Presenter : IListener
{
    private UISubItem_Quest _view;

    private Quest _quest;

    private UIPopup_QuestList _parent;

    public UISubItem_Quest_Presenter(UISubItem_Quest view)
    {
        _view = view;

        SetButton();
    }

    public void SetButton()
    {
        _view.SetButton(OnClickQuestButton, OnClickCompleteButtonPostCompletedAndReward);
    }

    private void OnClickQuestButton()
    {
        _parent.Presenter.SelectQuest(_quest);
    }

    private CompletedAndRewardBody MakeCompletedAndRewardBody()
    {
        // 퀘스트 데이터 담기
        AddCompletedBody addCompletedbody = new AddCompletedBody
        {
            Id = _quest.QuestInfoData.Id,
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            IsDailyQuest = _quest.QuestInfoData.Category == Define.QuestCategory.Daily
        };

        // 알 데이터 담기
        List<Reward> RewardList = _quest.RewardList;
        string EggId = "";
        EggReward eggReward = RewardList.Find(x => x is EggReward) as EggReward;
        if (eggReward != null)
        {
            EggId = eggReward.RewardInfo.RewardItemId;
        }

        // 아이템 데이터 담기
        List<Reward> ItemRewardList = RewardList.FindAll(x => x is ItemReward);
        Item[] items = new Item[ItemRewardList.Count];
        if (ItemRewardList.Count != 0)
        {
            for (int i = 0; i < ItemRewardList.Count; i++)
            {
                items[i] = new Item
                {
                    ItemId = (ItemRewardList[i] as ItemReward).RewardInfo.RewardItemId,
                    Quantity = (ItemRewardList[i] as ItemReward).RewardInfo.Quantity
                };
            }
        }

        // 리워드 데이터 담기
        RewardBody rewardBody = new RewardBody
        {
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            EggId = EggId,
            Items = items
        };

        // 경험치 데이터 담기
        ExpReward expReward = RewardList.Find(x => x is ExpReward) as ExpReward;
        int exp = 0;
        if (expReward != null)
        {
            exp = expReward.RewardInfo.Quantity;
        }

        // 유저 시스템에서 하는 코드 그냥 여기서 한다
        ServerUserGameInfoData _userData = DataManager.Instance.ServerDataSystem.User;
        _userData.Exp += exp;

        while (true)
        {
            if (_userData.Level == GameManager.Instance.UserSystem.MaxLevel && exp != 0)
            {
                Logging.LogError("UserLevelUpSystem::LevelUpCheck - PlayerLevelUp is null 최고레벨");
                break;
            }

            if (_userData.Exp < DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_userData.Level].SumOfExp)
            {
                break;
            }

            // 이전 레벨 저장
            if (GameManager.Instance.UserSystem.TempPrevLevel == -1)
            {
                GameManager.Instance.UserSystem.TempPrevLevel = _userData.Level;
            }

            _userData.Exp -= DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_userData.Level].SumOfExp;
            _userData.Level++;

            Logging.Log($"UserLevelUpSystem::LevelUpCheck {_userData.Level}");
        }

        // 경험치 데이터 담기
        LevelUpBody levelUpBody = new LevelUpBody
        {
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            Exp = DataManager.Instance.ServerDataSystem.User.Exp,
            Level = DataManager.Instance.ServerDataSystem.User.Level
        };

        return new CompletedAndRewardBody
        {
            CompletedQuest = addCompletedbody,
            Reward = rewardBody,
            Exp = levelUpBody
        };
    }

    public async void OnClickCompleteButtonPostCompletedAndReward()
    {
        CompletedAndRewardBody completedAndRewardBody = MakeCompletedAndRewardBody();

        await LoadingManager.Instance.RunWithLoading(async () =>
        {
            // 퀘스트 서버에 완료 요청, 리워드, 경험치도 같이 보내기
            await DataManager.Instance.ServerDataSystem.PostCompletedAndReward(
                completedAndRewardBody
                );
        },
       onLoaded: () =>
       {
           _quest.CompleteQuest();

           _parent.Presenter.SelectFirstQuest();

           // 리워드 안내 팝업
           UIPopup_QuestReward popup = UIManager.Instance.InstanciateUIPopup<UIPopup_QuestReward>();
           popup.Init(_quest.RewardList);

           // 레벨 업 팝업
           if (GameManager.Instance.UserSystem.TempPrevLevel != -1)
           {
               UIPopup_Warning popuplevelup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
               popuplevelup.SetText("레벨 업 " + GameManager.Instance.UserSystem.TempPrevLevel + " -> " + DataManager.Instance.ServerDataSystem.User.Level);
               GameManager.Instance.UserSystem.TempPrevLevel = -1;
           }

           _view.Destroy();
       }
       ,
       onError: (error) =>
       {
           Logging.LogError(error);
       });
    }

    /// <summary>
    /// 여기서 퀘스트 완료 버튼 활성화 해주기
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="parent"></param>
    public void SetQuest(Quest quest, UIPopup_QuestList parent)
    {
        _quest = quest;
        _parent = parent;

        EventManager.Instance.AddListener(EventType.OnWaitingForCompletion, this);

        _view.CompleteButton.gameObject.SetActive(false);
        if (_quest.State == Define.QuestState.WaitingForCompletion)
        {
            _view.CompleteButton.gameObject.SetActive(true);
        }

        _view.SetText(quest.QuestInfoData.Name);
    }

    /// <summary>
    /// 퀘스트 창에서 띄워놓고 있는 상태에서만 버튼 상태 바꿔주기
    /// </summary>
    /// <param name="quest"></param>
    private void WaitingForCompletion(Quest quest)
    {
        if (_view.CompleteButton == null)
        {
            return;
        }

        _view.CompleteButton.gameObject.SetActive(true);
    }

    public void OnEvent(EventType eventType, object sender, params object[] paramObjects)
    {
        if(eventType == EventType.OnWaitingForCompletion)
        {
            WaitingForCompletion((Quest)sender);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
/// <summary>
/// 퀘스트
/// 퀘스트는 테스크 그룹 리스트로 이루어져 있고, 테스크 그룹은 테스크 리스트로 이루어져 있다.
/// </summary>
public class Quest
{
    #region Events
    public event Action<Quest, Subtask, int, int> OnSubtaskSuccessChanged;
    public event Action<Quest> OnQuestCompleted;
    /*public event Action<Quest> OnCanceled;*/
    public event Action<Quest, SubtaskGroup, SubtaskGroup> OnNewTaskGroup;
    #endregion

    [Header("Subtask")]
    private List<SubtaskGroup> _subtaskGroupList;
    // TODO : 태스크 그룹 여러 개 만들면 CSV 파일도 인덱스 넣도록 변경하기
    private int _currentTaskGroupIndex;

    [Header("Option")]
    private bool _useAutoComplete = false;

    [Header("Reward")]
    private List<Reward> _rewardList;

    public QuestInfoData QuestInfoData { get; set; }
    public QuestState State { get; private set; }
    public SubtaskGroup CurrentSubtaskGroup => _subtaskGroupList[_currentTaskGroupIndex];
    public List<Reward> RewardList => _rewardList;

    public Quest(QuestInfoData data)
    {
        // 퀘스트 정보 데이터
        QuestInfoData = data;

        // 서브태스크그룹 리스트 초기화
        _subtaskGroupList = new List<SubtaskGroup>();
        SubtaskGroup suptaskgroup = new SubtaskGroup(this, DataManager.Instance.ReadOnlyDataSystem.Quest[QuestInfoData.Id].TaskDic);
        _subtaskGroupList.Add(suptaskgroup);

        _currentTaskGroupIndex = 0;

        // 리워드 종류 각 타입으로 생성할 때부터 하위 클래스로 만들기
        _rewardList = new List<Reward>();
        Dictionary<string, RewardInfoData> rewardDatabase = DataManager.Instance.ReadOnlyDataSystem.Quest[QuestInfoData.Id].RewardDic;
        foreach (RewardInfoData rewardData in rewardDatabase.Values)
        {
            if(rewardData.RewardItemId.Contains("ITE"))
            {
                ItemReward itemReward = new ItemReward(rewardData);
                _rewardList.Add(itemReward);
            }
            else if(rewardData.RewardItemId.Contains("EXP"))
            {
                ExpReward expReward = new ExpReward(rewardData);
                _rewardList.Add(expReward);
            }
            else if(rewardData.RewardItemId.Contains("EGG"))
            {
                EggReward eggReward = new EggReward(rewardData);
                _rewardList.Add(eggReward);
            }
        }
    }

    /// <summary>
    /// 퀘스트 등록되면 하위 테스크들도 등록해준다.
    /// </summary>
    public void RegisterQuest()
    {
        // 서브테스크 세팅하기
        foreach (SubtaskGroup taskGroup in _subtaskGroupList)
        {
            foreach (Subtask task in taskGroup.SubtaskList)
            {
                task.OnSubtaskSuccessChanged += ChangeSubtaskSuccess;
            }
        }

        // 퀘스트 진행중
        State = QuestState.Running;
        CurrentSubtaskGroup.StartSubtaskGroup();
    }

    /// <summary>
    /// 밑에 Subtask에서 받은 걸로 Quest에서 이벤트 Invoke, UI에 알려주기
    /// </summary>
    /// <param name="task"></param>
    /// <param name="currentSuccess"></param>
    /// <param name="prevSuccess"></param>
    private void ChangeSubtaskSuccess(Subtask task, int currentSuccess, int prevSuccess)
    {
        OnSubtaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);
    }

    /// <summary>
    /// 받은 리포트를 서브태스크 그룹에게 전달
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(QuestCategory category, object target, int successCount)
    {
        if (State == QuestState.Complete)
        {
            return;
        }

        // 서브태스크 그룹에게 리포트 보내기
        CurrentSubtaskGroup.ReceiveReport(category, target, successCount);

        // 현재 서브태스크 그룹 안에 모든 서브태스크가 완료
        if(CurrentSubtaskGroup.IsAllSubTaskComplete)
        {
            // 마지막 서브태스크 그룹이면 퀘스트 완료
            if (_currentTaskGroupIndex + 1 == _subtaskGroupList.Count)
            {
                State = QuestState.WaitingForCompletion;

                EventManager.Instance.Invoke(EventType.OnWaitingForCompletion, this);

                if (_useAutoComplete)
                {
                    CompleteQuest();
                }
            }
            // 다음 서브테스크 그룹으로 넘어가기
            else
            {
                SubtaskGroup prevTaskGroup = CurrentSubtaskGroup;
                _currentTaskGroupIndex++;

                prevTaskGroup.End();
                CurrentSubtaskGroup.StartSubtaskGroup();

                OnNewTaskGroup?.Invoke(this, prevTaskGroup, CurrentSubtaskGroup);
            }
        }
    }

    /// <summary>
    /// 보상받기 버튼 누르면 호출
    /// </summary>
    public void CompleteQuest()
    {
        // 가장 밑 서브테스크까지 Complete로 변경
        foreach(SubtaskGroup subtaskGroup in _subtaskGroupList)
        {
            subtaskGroup.CompleteSubtaskGroup();
        }

        State = QuestState.Complete;

        // 리워드 주기
        foreach (Reward reward in _rewardList)
        {
            reward.Give(this);
        }

        OnQuestCompleted?.Invoke(this);

        OnSubtaskSuccessChanged = null;
        OnQuestCompleted = null;
        /*OnCanceled = null;*/
        OnNewTaskGroup = null;
        OnSubtaskSuccessChanged = null;
    }
}

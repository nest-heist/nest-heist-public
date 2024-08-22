using System;
using System.Linq;
using UnityEngine;
using static Define;

/// <summary>
/// </summary>
public class Subtask
{
    #region Events
    // TODO : OnSubtaskStateChanged 지금 당장은 안쓰는데 쓸 일 생길지도
    /*public event Action<Subtask, SubtaskState, SubtaskState> OnSubtaskStateChanged;*/
    public event Action<Subtask, int, int> OnSubtaskSuccessChanged;
    #endregion

    [Header("Setting")]
    public int _initialSuccessValue;

    /// <summary>
    /// TODO : 상호참조 문제 안생기는지 확인하기
    /// </summary>
    public Quest Owner { get; private set; }

    // 퀘스트 카테고리는 파싱할 때 퀘스트데이터에 담겨있다 
    public QuestCategory QuestCategory => Owner.QuestInfoData.Category;
    public bool IsComplete => State == SubtaskState.Complete;

    public SubtaskInfoData TaskInfoData { get; set; }

    private int _currentSuccess;

    /// <summary>
    /// 횟수 바뀌면 ui 바꾸기
    /// 이벤트로 위 계층인 quest에 알려주기
    /// </summary>
    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, TaskInfoData.NeedSuccessToCompleted);

            if (prevSuccess != _currentSuccess)
            {
                State = _currentSuccess == TaskInfoData.NeedSuccessToCompleted ? SubtaskState.Complete : SubtaskState.Running;
                OnSubtaskSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
        }
    }

    private SubtaskState _state;

    /// <summary>
    /// Subtask의 State 바뀌면 이벤트로 위 계층인 quest에 알려주기
    /// </summary>
    public SubtaskState State
    {
        get => _state;
        private set
        {
            SubtaskState prevState = _state;
            _state = value;

            /*if(prevState != _state)
            {
                OnSubtaskStateChanged?.Invoke(this, _state, prevState);
            }*/
        }
    }

    /// <summary>
    /// 생성할 때 데이터 넣어주기
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="taskInfoData"></param>
    public Subtask(Quest owner, SubtaskInfoData taskInfoData)
    {
        Owner = owner;
        TaskInfoData = taskInfoData;
        CurrentSuccess = 0;
    }

    /// <summary>
    /// 서버에서 받은 값으로 시작
    /// </summary>
    /// <param name="initialSuccessValue"></param>
    public void StartSubtask(int initialSuccessValue)
    {
        State = SubtaskState.Running;

        CurrentSuccess += initialSuccessValue;
    }

    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = TaskInfoData.SuccessCountProcessor.Run(this, CurrentSuccess, successCount);
    }

    public bool IsTarget(QuestCategory category, object target)
    {
        // 카테고리 체크
        if (QuestCategory != category)
        {
            return false;
        }

        // 모든 타겟 중 하나도 아니면 false
        if (TaskInfoData.TargetList.Any(x => x.IsEqual(target)) == false)
        {
            return false;
        }

        if (IsComplete)
        {
            return false;
        }

        return true;
    }

    public void End()
    {
        /*OnSubtaskStateChanged = null;*/
        OnSubtaskSuccessChanged = null;
    }

    public void CompleteSubtask()
    {
        CurrentSuccess = TaskInfoData.NeedSuccessToCompleted;
    }
}

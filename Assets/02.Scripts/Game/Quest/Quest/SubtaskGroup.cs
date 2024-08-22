using System;
using System.Collections.Generic;
using System.Linq;
using static Define;

/// <summary>
/// 테스크 여러개를 묶어 놓았다
/// </summary>
public class SubtaskGroup
{
    private List<Subtask> _subtaskList;

    public List<Subtask> SubtaskList => _subtaskList;
    public Quest Owner { get; private set; }
    public Define.SubtaskGroupState State { get; private set; }

    public bool IsAllSubTaskComplete => _subtaskList.All(x => x.IsComplete);


    public SubtaskGroup(Quest quest, Dictionary<string, SubtaskInfoData> taskDatabase)
    {
        Owner = quest;

        _subtaskList = new List<Subtask>();

        // 서브테스크 만들 때 서버 값 가지고 넣어주기
        foreach (SubtaskInfoData taskData in taskDatabase.Values)
        {
            Subtask task = new Subtask(quest, taskData);
            _subtaskList.Add(task);
        }
    }

    /// <summary>
    /// 서버에서 Subtask Id로 받아온 데이터로 서브태스크 처음 값 넣어서 시작하기
    /// </summary>
    public void StartSubtaskGroup()
    {
        State = SubtaskGroupState.Running;

        Dictionary<string, ServerSubtaskData> ServerSubtaskDic = GameManager.Instance.QuestSystem.ServerSubtaskDic;
        foreach (Subtask task in _subtaskList)
        {
            if (ServerSubtaskDic.ContainsKey(task.TaskInfoData.Id) == true)
            {
                task.StartSubtask(ServerSubtaskDic[task.TaskInfoData.Id].Value);
            }
        }
    }

    /// <summary>
    /// 각 서브태스크가 카테고리랑 대상이 맞으면 횟수 올리기
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(QuestCategory category, object target, int successCount)
    {
        foreach (Subtask subtask in _subtaskList)
        {
            if (subtask.IsTarget(category, target))
            {
                subtask.ReceiveReport(successCount);
            }
        }
    }

    public void CompleteSubtaskGroup()
    {
        if (State == SubtaskGroupState.Complete)
        {
            return;
        }

        State = SubtaskGroupState.Complete;

        foreach (Subtask subtask in _subtaskList)
        {
            if (subtask.IsComplete == false)
            {
                subtask.CompleteSubtask();
            }
        }
    }

    public void End()
    {
        State = SubtaskGroupState.Complete;
        foreach (Subtask subtask in _subtaskList)
        {
            subtask.End();
        }
    }
}

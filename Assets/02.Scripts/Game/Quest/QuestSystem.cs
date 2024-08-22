using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 전체 퀘스트 컨트롤하는 매니저
/// </summary>
public class QuestSystem : IListener
{
    #region Events
    public event Action<Quest> OnQuestRegistered;
    public event Action<Quest> OnQuestCompleted;
    public event Action<Quest> OnQuestCanceled;
    #endregion

    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();
    // 퀘스트 주는 시작점 리스트, 전체 퀘스트 다 들고있다 
    private List<Quest> _allQuestList;

    public List<Quest> ActiveQuests => _activeQuests;

    // 하위 서브테스크에서 여러 번 호출 X, 캐싱하기
    public Dictionary<string, ServerSubtaskData> ServerSubtaskDic { get; private set; }

    /// <summary>
    /// 서버에서 데이터 갖고와서 필요한 것만 ActiveQuests에 넣기
    /// </summary>
    public void Init()
    {
        EventManager.Instance.AddListener(EventType.OnMonsterDead, this);
        EventManager.Instance.AddListener(EventType.OnEnterPortal, this);

        EventManager.Instance.AddListener(EventType.OnUseSmallNutrient, this);
        EventManager.Instance.AddListener(EventType.OnUseMediumNutrient, this);
        EventManager.Instance.AddListener(EventType.OnUseLargeNutrient, this);
        EventManager.Instance.AddListener(EventType.OnEndHatching, this);

        EventManager.Instance.AddListener(EventType.OnGetGold, this);
        EventManager.Instance.AddListener(EventType.OnLogin, this);

        _allQuestList = new List<Quest>();

        MakeAllQuestListFromCSV();
    }

    /// <summary>
    /// 전체 퀘스트 만들기
    /// </summary>
    private void MakeAllQuestListFromCSV()
    {
        Dictionary<string, QuestInfoData> questDataBase = DataManager.Instance.ReadOnlyDataSystem.Quest;
        ServerSubtaskDic = DataManager.Instance.ServerDataSystem.SubtaskList.ToDictionary(x => x.Id);

        foreach (QuestInfoData data in questDataBase.Values)
        {
            Quest quest = new Quest(data);
            _allQuestList.Add(quest);
        }

        RegisterAciveQuests();
    }

    /// <summary>
    /// 완료된 퀘스트리스트에서 찾아서 전체퀘스트 리스트에서 없애야 한다
    /// 완료된퀘스트 리스트 -> 딕셔너리로 바꾸기, 전체 퀘스트에서 필요한 것만 등록하기
    /// </summary>
    private void RegisterAciveQuests()
    {
        List<ServerCompletedQuestData> completedQuestList = DataManager.Instance.ServerDataSystem.CompletedQuestDataList;
        Dictionary<string, ServerCompletedQuestData> completedQuestDic = completedQuestList.ToDictionary(x => x.Id);

        foreach (Quest quest in _allQuestList)
        {
            if (completedQuestDic.ContainsKey(quest.QuestInfoData.Id) == false)
            {
                this.Register(quest);
            }
        }
    }

    /// <summary>
    /// 퀘스트 받아서 등록
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public Quest Register(Quest quest)
    {
        quest.OnQuestCompleted += QuestCompleted;
        /*quest.OnCanceled += QuestCanceled;*/
        quest.OnSubtaskSuccessChanged += PostSubtaskSuccessChange;

        _activeQuests.Add(quest);

        quest.RegisterQuest();
        OnQuestRegistered?.Invoke(quest);

        return quest;
    }

    /// <summary>
    /// 서브테스크 성공 횟수 서버에 전달
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="subtask"></param>
    /// <param name="currentSuccess"></param>
    /// <param name="prevSuccess"></param>
    private void PostSubtaskSuccessChange(Quest quest, Subtask subtask, int currentSuccess, int prevSuccess)
    {
        DataManager.Instance.ServerDataSystem.UpdateSubtaskCurrenSuccessValue(
                new UpdateValueBody
                {
                    Id = subtask.TaskInfoData.Id,
                    QuestId = quest.QuestInfoData.Id,
                    UserId = DataManager.Instance.ServerDataSystem.User.UserId,
                    Value = currentSuccess,
                    IsDailyQuest = quest.QuestInfoData.Category == Define.QuestCategory.Daily
                }
            );
    }

    /// <summary>
    /// TODO : 업적 생기면 추가하기
    /// 현재는 퀘스트만 리포트 받음
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(Define.QuestCategory category, object target, int successCount = 1)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
    }

    /// <summary>
    /// 각 퀘스트에게 리포트 보내기
    /// 그럼 그 하위에서 알아서 처리
    /// </summary>
    /// <param name="quests"></param>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    private void ReceiveReport(List<Quest> quests, Define.QuestCategory category, object target, int successCount)
    {
        foreach (Quest quest in quests)
        {
            quest.ReceiveReport(category, target, successCount);
        }
    }

    #region 콜백 처리

    /// <summary>
    /// TODO : Destory 처리 필요할까? 
    /// </summary>
    /// <param name="quest"></param>
    private void QuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);
        OnQuestCanceled?.Invoke(quest);

        /*Destroy(quest, Time.deltaTime);*/
    }

    private void QuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        OnQuestCompleted?.Invoke(quest);
    }

    #endregion

    /// <summary>
    /// 여러 곳에서 온 퀘스트 완료 리포트 받기
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="sender"></param>
    /// <param name="paramObjects"></param>
    public void OnEvent(EventType eventType, object sender, params object[] paramObjects)
    {
        switch (eventType)
        {
            case EventType.OnMonsterDead:
                ReceiveReport(Define.QuestCategory.Daily, Define.TargetMonster);
                ReceiveReport(Define.QuestCategory.Dungeon, Define.TargetMonster);
                break;

            case EventType.OnEnterPortal:
                ReceiveReport(Define.QuestCategory.Dungeon, Define.TargetPortal);
                break;

            case EventType.OnUseSmallNutrient:
                ReceiveReport(Define.QuestCategory.Growth, Define.TargetSmallNutrient, (int)paramObjects[0]);
                break;

            case EventType.OnUseMediumNutrient:
                ReceiveReport(Define.QuestCategory.Growth, Define.TargetMediumNutrient, (int)paramObjects[0]);
                break;

            case EventType.OnUseLargeNutrient:
                ReceiveReport(Define.QuestCategory.Growth, Define.TargetLargeNutrient, (int)paramObjects[0]);
                break;

            case EventType.OnEndHatching:
                ReceiveReport(Define.QuestCategory.Growth, Define.TargetEgg);
                break;

            case EventType.OnGetGold:
                ReceiveReport(Define.QuestCategory.Daily, Define.TargetGold, (int)paramObjects[0]);
                break;

            case EventType.OnLogin:
                ReceiveReport(Define.QuestCategory.Daily, Define.TargetLogin);
                ReceiveReport(Define.QuestCategory.Growth, Define.TargetLogin);
                break;

        }

        Logging.Log($"QuestSystem::OnEvent() eventType: {eventType}");
    }
}

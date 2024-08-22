using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EventType
{
    // 퀘스트 컨텐츠
    OnMonsterDead,
    OnEnterPortal,

    OnUseSmallNutrient,
    OnUseMediumNutrient,
    OnUseLargeNutrient,
    OnEndHatching,

    OnGetGold,
    OnLogin,

    // 사운드
    OnSetVolume,

    // 퀘스트 완료처리
    OnWaitingForCompletion,
}

/// <summary>
/// 리스너 인터페이스
/// 이벤트가 발생했을 때 처리를 각 클래스에서 정의
/// </summary>
public interface IListener
{
    void OnEvent(EventType eventType, object sender, params object[] paramObjects);
}

/// <summary>
/// 중앙에서 이벤트 관리하는 매니저
/// 모든 리스너를 IListener 타입으로 관리
/// </summary>
public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventType, List<IListener>> _listenerDic = new Dictionary<EventType, List<IListener>>();

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    /// <summary>
    /// 이벤트 타입에 해당하는 리스너리스트에 리스너 추가 
    /// 중복 추가 가능, 추가할 때 주의하기
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listener"></param>
    public void AddListener(EventType eventType, IListener listener)
    {
        List<IListener> listenerList = null;

        if (_listenerDic.TryGetValue(eventType, out listenerList) == true)
        {
            listenerList.Add(listener);
            return;
        }

        // 이벤트 타입에 해당하는 리스너 리스트 없을 때 새로 만들어서 추가
        listenerList = new List<IListener> { listener };
        _listenerDic.Add(eventType, listenerList);
    }

    /// <summary>
    /// 이벤트 타입에 해당하는 리스너 리스너리스트에서 제거
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listener"></param>
    public void RemoveListener(EventType eventType, IListener listener)
    {
        List<IListener> ListenerList = null;

        if (_listenerDic.TryGetValue(eventType, out ListenerList))
        {
            ListenerList.Remove(listener);

            // 리스너 리스트가 비었다면 이벤트 타입 제거
            if (ListenerList.Count == 0)
            {
                _listenerDic.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 리스너에게 이벤트를 알린다. 
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="sender"></param>
    /// <param name="paramObjects"></param>
    public void Invoke(EventType eventType, object sender, params object[] paramObjects)
    {
        List<IListener> listenerList = null;

        // 이벤트 타입에 해당하는 리스너 리스트가 없다면 리턴
        if (_listenerDic.TryGetValue(eventType, out listenerList) == false)
        {
            return;
        }

        // OnEvent 돌면서 불러주기
        foreach (IListener listener in listenerList)
        {
            if (listener.Equals(null) == false)
            {
                listener.OnEvent(eventType, sender, paramObjects);
            }
        }
    }

    /// <summary>
    /// 이벤트 제거
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveEvent(EventType eventType)
    {
        _listenerDic.Remove(eventType);
    }

    /// <summary>
    /// null인 리스너 제거
    /// </summary>
    public void RemoveNullListener()
    {
        Dictionary<EventType, List<IListener>> tempListenerDic = new Dictionary<EventType, List<IListener>>();

        foreach (KeyValuePair<EventType, List<IListener>> item in _listenerDic)
        {
            // 널인 리스너 제거, 리스트여서 뒤에서부터 제거
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i].Equals(null))
                {
                    item.Value.RemoveAt(i);
                }
            }

            // 널 아닌 것만 넣어주기
            if (item.Value.Count > 0)
            {
                tempListenerDic.Add(item.Key, item.Value);
            }
        }

        _listenerDic = tempListenerDic;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 씬 로드시 널인 리스너 전부 제거
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RemoveNullListener();
    }
}

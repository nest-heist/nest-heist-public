using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System.Net;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;


[TaskCategory("Boss001")]
public class RandomRotateWindPattern : Action
{
    //회전바람 프리팹
    [Header("Objects")]
    public GameObject RotateWind;
    public GameObject Center;
    public List<RotateWind> RotateWinds;
    public List<GameObject> CenterList;
    public SharedGameObjectList SharedCenterList;

    [Header("Time")]
    private float _waitDuration = 5f;
    private float _startTime;
    private float _pauseTime;

    float x;
    float y;

    public SharedMON10001 Boss;

    [Header("Count")]
    public SharedInt SharedRotateWindCnt;
    public int RotateWindCount;
    //매번 랜덤으로 생성해서 Rotate Wind를 만들때 Init()해줌

    public override void OnAwake()
    {
        RotateWinds = new List<RotateWind>();
        RotateWindCount = SharedRotateWindCnt.Value;
    }

    public override void OnStart()
    {
        _startTime = Time.time;
        CreateRandomCenter();
        CreateWind();
    }

    public override TaskStatus OnUpdate()
    {
        if (_startTime + _waitDuration < Time.time)
        {
            return TaskStatus.Success;
        }
        // Otherwise we are still waiting.
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        for (int i = 0; i < CenterList.Count; i++)
        {
            RotateWinds[i].ReleaseObject();
            SharedCenterList.Value[i].GetComponent<PoolAble>().ReleaseObject();
        }
        CenterList.Clear();
        RotateWinds.Clear();
        SharedCenterList.Value.Clear();
    }

    private void CreateRandomCenter()
    {
        CenterList = SharedCenterList.Value;
    }
    private void CreateWind()
    {
        for (int i = 0; i < RotateWindCount; i++)
        {
            RotateWind obj = PoolManager.Instance.GetGameObject("RotateWind").GetComponent<RotateWind>();
            obj.InitDamage(Boss.Value.StatHandler.CurrentStat.Attack);
            obj.InitCenter(CenterList[i].transform);
            RotateWinds.Add(obj);
        }
    }

    public override void OnPause(bool paused)
    {
        if (paused)
        {
            // Remember the time that the behavior was paused.
            _pauseTime = Time.time;
        }
        else
        {
            // Add the difference between Time.time and pauseTime to figure out a new start time.
            _startTime += (Time.time - _pauseTime);
        }
    }

    public override void OnReset()
    {
        // Reset the public properties back to their original values
        _waitDuration = 5f;
    }
}
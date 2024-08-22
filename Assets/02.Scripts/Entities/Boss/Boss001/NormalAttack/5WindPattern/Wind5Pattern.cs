using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using DG.Tweening.Core;
using Unity.Mathematics;

/// <summary>
/// 5갈래 방향으로 바람을 소환해서 날림
/// </summary>
/// 
[TaskCategory("Boss001")]
public class Wind5Pattern : Action
{
    [Header("Object")]
    private string _windName = "Wind";
    public GameObject Wind;
    //바람이 시작되는 위치
    public List<Wind> WindList;

    [Header("wind")]
    private float _rotZ;
    private float _rotValue;
    public SharedTransform WindPoint;
    public SharedInt WindCount;
    private int _windCount;

    // The time to wait
    [Header("Time")]
    private float _waitDuration;
    private float _startTime;
    private float _pauseTime;
    public SharedFloat waitTime = 6;

    public SharedMON10001 Boss;

    public override void OnStart()
    {
        _startTime = Time.time;
        _waitDuration = waitTime.Value;
        _rotValue = 180 / (WindCount.Value + 1);

        _windCount = WindCount.Value;
        WindList = new List<Wind>();
        _rotZ = 90f;
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
    private void CreateWind()
    {
        for (int i = 0; i < _windCount; i++)
        {
            Wind obj = PoolManager.Instance.GetGameObject(_windName).GetComponent<Wind>();
            obj.InitDamage(Boss.Value.StatHandler.CurrentStat.Attack * 2);
            obj.transform.position = WindPoint.Value.transform.position;
            WindList.Add(obj);
            _rotZ += _rotValue;
            obj.transform.localEulerAngles = new Vector3(0, 0, _rotZ);
            obj.dir = Quaternion.Euler(0, 0, _rotZ) * transform.right;
        }
    }
    public override void OnEnd()
    {
        for (int i = 0; i < _windCount; i++)
        {
            WindList[i].ReleaseObject();
        }
        WindList.Clear();
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
        waitTime = 4;
    }
}

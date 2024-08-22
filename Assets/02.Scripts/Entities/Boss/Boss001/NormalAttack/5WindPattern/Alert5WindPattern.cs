using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

/// <summary>
/// 2초동안 깜빡여 공격 위치를 미리 알려줌
/// </summary>
[TaskCategory("Boss001")]
public class Alert5WindPattern : Action
{
    [Header("ObjectList")]
    private string _gizmoName = "Gizmo";
    //public List<PoolAble> GizmoList;
    //public List<Renderer> GizmoRendererList;
    public List<PoolAble> GizmoList = new List<PoolAble>();
    public List<Renderer> GizmoRendererList = new List<Renderer>();

    [Header("Transform")]
    public SharedTransform WindPoint;
    private float _rotZ;
    private float _rotValue;

    [Header("Count")]
    public SharedInt WindCount;
    private int _windCount;

    [Header("Blink Time")]
    private float _blinkTime = 0.1f;
    private float _nextBlinkTime;
    private bool _isBlinking;

    [Header("Pattern Time")]
    private float _waitDuration;
    private float _startTime;

    public override void OnAwake()
    {
        _windCount = WindCount.Value;
    }
    public override void OnStart()
    {
        _rotZ = -90f;
        _rotValue = 180 / (WindCount.Value + 1);
        _startTime = Time.time;
        _waitDuration = 2.5f;

        CreateGizmo();
        SetGizmoRenderer();
        _nextBlinkTime = Time.time + _blinkTime;
        _isBlinking = true;
    }
    public override TaskStatus OnUpdate()
    {
        if (Time.time >= _nextBlinkTime)
        {
            _isBlinking = !_isBlinking;
            for (int i = 0; i < GizmoList.Count; i++)
            {
                GizmoRendererList[i].enabled = _isBlinking;
            }
            _nextBlinkTime = Time.time + _blinkTime;
        }

        if (_startTime + _waitDuration < Time.time)
        {
            return TaskStatus.Success;
        }
        //Running으로 비동기 작업 처리
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        for (int i = 0; i < GizmoList.Count; i++)
        {
            GizmoList[i].ReleaseObject();
        }
        GizmoList.Clear();
        GizmoRendererList.Clear();
    }

    public override void OnReset()
    {
        // Reset the public properties back to their original values
        _waitDuration = 2f;
    }

    public void CreateGizmo()
    {
        for (int i = 0; i < _windCount; i++)
        {
            //GameObject obj = PoolManager.Instance.GetGameObject("Gizmo");
            //GizmoList.Add(obj.GetComponent<PoolAble>());
            //_rotZ += _rotValue;
            //obj.transform.localEulerAngles = new Vector3(0, 0, _rotZ);

            GameObject obj = PoolManager.Instance.GetGameObject(_gizmoName);
            if (obj != null)
            {
                PoolAble poolAble = obj.GetComponent<PoolAble>();
                if (poolAble != null)
                {
                    GizmoList.Add(poolAble);
                    _rotZ += _rotValue;
                    obj.transform.localEulerAngles = new Vector3(0, 0, _rotZ);
                }
                else
                {
                    Logging.LogError("Gizmo에 PoolAble 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Logging.LogError($"풀에서 {_gizmoName}을(를) 찾을 수 없습니다.");
            }
        }
    }

    private void SetGizmoRenderer()
    {
        for (int i = 0; i < GizmoList.Count; i++)
        {
            //  GizmoRendererList.Add(GizmoList[i].GetComponentInChildren<Renderer>());
            Renderer renderer = GizmoList[i]?.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                GizmoRendererList.Add(renderer);
            }
            else
            {
                Logging.LogError("Gizmo의 자식 객체에서 Renderer 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }
}

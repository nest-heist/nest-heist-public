using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using Unity.VisualScripting;

[TaskCategory("Boss001")]
public class AlertRandomRotateWind : Action
{
    public SharedGameObjectList SharedCenterList;
    public SharedMON10001 Boss;

    public SharedInt SharedRotateWindCnt;
    public int RotateWindCount;

    [Header("Prefabs")]
    public GameObject Gizmo;
    public GameObject Center;

    //public List<GameObject> GizmoList;
    public List<GameObject> GizmoList;
    public List<Renderer> GizmoRendererList;

    private Vector2 _pos;

    private float _blinkTime = 0.1f;
    private float _nextBlinkTime;
    private bool _isBlinking;

    private float _waitDuration = 3f;
    private float _startTime;


    float x, y;
    public override void OnAwake()
    {
        RotateWindCount = SharedRotateWindCnt.Value;
        GizmoList = new List<GameObject>();
        GizmoRendererList = new List<Renderer>();
    }

    public override void OnStart()
    {
        if (Boss == null || Boss.Value == null)
        {
            Logging.LogError("Boss 또는 Boss.Value가 null입니다.");
            return;
        }

        if (SharedCenterList == null || SharedCenterList.Value == null)
        {
            Logging.LogError("SharedCenterList 또는 SharedCenterList.Value가 null입니다.");
            return;
        }
        CreateRandomCenter();
        AddGizmoRenderer();
        _startTime = Time.time;
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
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        for (int i = 0; i < RotateWindCount; i++)
        {
            GizmoList[i].GetComponent<PoolAble>().ReleaseObject();
            //GizmoRendereList[i].Destory();
        }
        GizmoList.Clear();
        GizmoRendererList.Clear();
    }
    private void CreateRandomCenter()
    {
        for (int i = 0; i < RotateWindCount; i++)
        {
            /*GameObject obj = GameObject.Instantiate(Center);
            GameObject gizmo = GameObject.Instantiate(Gizmo);*/
            GameObject obj = PoolManager.Instance.GetGameObject("Center");
            GameObject gizmo = PoolManager.Instance.GetGameObject("RotateGizmo");
            if (obj == null)
            {
                Logging.LogError("obj가 null입니다.");
                continue;
            }
            if (gizmo == null)
            {
                Logging.LogError("gizmo가 null입니다.");
                continue;
            }

            x = Random.Range(Boss.Value.TopLeft.x, Boss.Value.TopRight.x);
            y = Random.Range(Boss.Value.BottomLeft.y, Boss.Value.TopLeft.y);
            _pos.x = x;
            _pos.y = y;
            obj.transform.position = _pos;
            gizmo.transform.position = _pos;
            SharedCenterList.Value.Add(obj);
            GizmoList.Add(gizmo);
        }
    }

    private void AddGizmoRenderer()
    {
        for (int i = 0; i < GizmoList.Count; i++)
        {
            //  GizmoRendererList.Add(GizmoList[i].GetComponent<Renderer>());

            Renderer renderer = GizmoList[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                GizmoRendererList.Add(renderer);
            }
            else
            {
                Logging.LogError("GizmoList[" + i + "]에 Renderer가 없습니다.");
            }
        }
    }
}
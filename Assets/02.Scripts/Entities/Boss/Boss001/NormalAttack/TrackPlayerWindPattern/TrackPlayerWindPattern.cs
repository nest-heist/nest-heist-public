using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;
using System.Collections.Generic;

[TaskCategory("Boss001")]
public class TrackPlayerWindPattern : Action
{
    [Header("Prefabs")]
    public GameObject Gizmo;
    public GameObject Wind;
    [Header("Lists")]
    public SharedGameObjectList SpawnPoint;
    public List<Wind> WindList;
    //플레이어
    public SharedTransform Target;
    [Header("WindTime")]
    public float WindSpawnTime = 0.5f;
    public float NextWindSpawnTime;
    public float GizmoSpawnTime = 1f;
    public float NextGizmoSpawnTime;
    //마지막 공을 날리고 대기하는 시간
    public float EndingTime = 3f;
    public float EndTime;
    public bool IsEnd = false;

    [SerializeField] private int idx;

    public SharedMON10001 Boss;

    public override void OnAwake()
    {
        base.OnAwake();
        WindList = new List<Wind>();
    }
    public override void OnStart()
    {
        idx = 0;
        NextWindSpawnTime = Time.time + WindSpawnTime;
    }
    public override TaskStatus OnUpdate()
    {
        if (idx >= SpawnPoint.Value.Count)
        {
            if (!IsEnd)
            {
                EndTime = Time.time;
                IsEnd = true;
            }
            if (EndTime + EndingTime <= Time.time)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
        if (Time.time > NextWindSpawnTime)
        {
            CreateTrackingWind();
            NextWindSpawnTime = Time.time + WindSpawnTime;
        }

        return TaskStatus.Running;
    }
    public override void OnEnd()
    {
        if(WindList.Count != 0)
        {
            for (int i = 0; i < WindList.Count; i++)
            {
                WindList[i].ReleaseObject();
            }
        }
        WindList.Clear();
        idx = 0;
        IsEnd = false;
    }
    public void CreateTrackingWind()
    {
        Wind obj = PoolManager.Instance.GetGameObject("Wind").GetComponent<Wind>();
        obj.transform.position = SpawnPoint.Value[idx].transform.position;
        //Wind obj = GameObject.Instantiate(Wind, SpawnPoint[idx].transform.position, Quaternion.identity).GetComponent<Wind>();
        obj.InitDamage(Boss.Value.StatHandler.CurrentStat.Attack * 2);
        obj.dir = (Target.Value.position - SpawnPoint.Value[idx].transform.position).normalized;
        WindList.Add(obj);
        idx++;
    }


}

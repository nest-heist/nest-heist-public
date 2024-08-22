using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[TaskCategory("Boss001")]
public class BreathePattern : Action
{
    [Header("Check Behind Tree")]
    private bool IsPlayerBehindStone;
    private bool IsPartnerBehindStone;

    [Header("Pattern Time")]
    public float PatternTime = 6f;
    private float _startTime;
    [Header("Damage Period")]
    public float TickTime = 0.5f;
    [SerializeField] private float _nextTick;

    [Header("Effetc")]
    public GameObject WindStorm;
    public SharedTransform WindStromTransform;

    public GameObject WindStormClone;

    public SharedMON10001 Boss;

    public override void OnStart()
    {
        _startTime = Time.time;
        _nextTick = Time.time + TickTime;
        WindStormClone = GameObject.Instantiate(WindStorm, WindStromTransform.Value.position, Quaternion.Euler(-90, 90, 90));
    }


    public override TaskStatus OnUpdate()
    {
        if (_startTime + PatternTime < Time.time)
        {
            return TaskStatus.Success;
        }
        if (_nextTick <= Time.time)
        {
            Breathe();
            _nextTick = Time.time + TickTime;
        }
        return TaskStatus.Running;
    }
    public override void OnEnd()
    {
        for (int i = 0; i < Boss.Value.Stones.Count; i++)
        {
            Boss.Value.Stones[i].ReleaseObject();
        }
        Boss.Value.Stones.Clear();
        GameObject.Destroy(WindStormClone);
    }

    private void Breathe()
    {
        CheckBehindStone();
        if (!IsPlayerBehindStone)
        {
            Boss.Value.Player.Health.TakeDamage(Boss.Value.StatHandler.CurrentStat.Attack * 10000);
        }
        /*if (!IsPartnerBehindStone)
        {
            //데미지 주기
            Boss.Value.CurPartner.HealthSystem.TakeDamage(Boss.Value.StatHandler.CurrentStat.Attack);
        }*/
    }
    private void CheckBehindStone()
    {
        IsPartnerBehindStone = false;
        IsPlayerBehindStone = false;
        for (int i = 0; i < Boss.Value.Stones.Count; i++)
        {
            if (!IsPartnerBehindStone)
            {
                IsPartnerBehindStone = Boss.Value.Stones[i].CheckPartnerBehind();
            }
            if (!IsPlayerBehindStone)
            {
                IsPlayerBehindStone = Boss.Value.Stones[i].CheckPlayerBehind();
            }
        }
    }
}

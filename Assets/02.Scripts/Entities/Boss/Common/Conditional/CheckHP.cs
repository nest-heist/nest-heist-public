using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss001")]
public class CheckHP : Conditional
{
    //현재 보스체력
    public SharedFloat healthThreshold;
    private BossHealthSystem _bossHealth;

    public override void OnAwake()
    {
        _bossHealth = GetComponent<BossHealthSystem>();
    }


    public override TaskStatus OnUpdate()
    {
        if (_bossHealth.GetHealthPercentage() <= healthThreshold.Value)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}

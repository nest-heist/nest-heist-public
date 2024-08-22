using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class IsTargetInRange : Conditional
{
    public float AttackRange;
    private GameObject _player;
    private NavMeshAgent _navMeshAgent;

    public override void OnAwake()
    {
        _player = BossComponents.Instance.Player;
        _navMeshAgent = BossComponents.Instance.NavMeshAgent;
    }

    public override TaskStatus OnUpdate()
    {
        if (_player == null)
        {
            return TaskStatus.Failure;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= AttackRange)
        {
            _navMeshAgent.isStopped = true;
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
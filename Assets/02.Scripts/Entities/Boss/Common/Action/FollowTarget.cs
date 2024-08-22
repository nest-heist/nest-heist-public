using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class FollowTarget : Action
{
    public float AttackRange;
    private GameObject _player;
    private GameObject _mainSprite;
    private NavMeshAgent _navMeshAgent;
    public float followDuration = 7.0f;  // 7초 동안 따라다니기
    private float _startTime;

    public override void OnAwake()
    {
        _player = BossComponents.Instance.Player;
        _mainSprite = BossComponents.Instance.MainSprite;
        _navMeshAgent = BossComponents.Instance.NavMeshAgent;
        _startTime = Time.time;  // 타이머 초기화

    }

    public override void OnStart()
    {
        _startTime = Time.time;  // 태스크가 시작될 때마다 타이머 초기화
    }

    public override TaskStatus OnUpdate()
    {
        if (_player == null)
        {
            return TaskStatus.Failure;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // 플레이어가 사거리 내에 있을 때 에이전트 멈추기
        if (distance <= AttackRange)
        {
            _navMeshAgent.isStopped = true;
            return TaskStatus.Success;
        }

        // 특정 시간(예: 7초)이 지나면 성공 반환
        if (Time.time - _startTime >= followDuration)
        {
            return TaskStatus.Success;
        }

        // 플레이어의 위치로 이동
        if (_navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
        }
        _navMeshAgent.SetDestination(_player.transform.position);
        Rotate();


        return TaskStatus.Running;
    }

    private void Rotate()
    {
        Vector2 dir = _navMeshAgent.velocity.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _mainSprite.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Abs(angle) > 90 ? 0 : 180, 0));
    }
}

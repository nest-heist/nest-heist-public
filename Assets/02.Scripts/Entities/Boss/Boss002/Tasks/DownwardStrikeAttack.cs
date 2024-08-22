using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class DownwardStrikeAttack : Action
{
    public float UpHeight = 15f; // 보스가 올라가는 높이
    public float MoveSpeed = 20f; // 보스가 이동하는 속도
    public float DamageRadius = 3.5f; // 피해를 주는 반경
    public LayerMask TargetLayer; // 타겟이 속한 레이어

    private GameObject _castAreaObject;
    private NavMeshAgent _navMeshAgent;

    private bool _isSkillRunning = false;
    private bool _skillCompleted = false;

    public override void OnAwake()
    {
        _navMeshAgent = BossComponents.Instance.NavMeshAgent;
    }
    public override void OnStart()
    {
        if (!_isSkillRunning)
        {
            _skillCompleted = false;
            StartCoroutine(SkillRoutine(BossComponents.Instance.Player.transform.position));
            _isSkillRunning = true;
        }
    }
    public override TaskStatus OnUpdate()
    {
        // 코루틴이 완료되었는지 확인
        if (_skillCompleted)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public void PerformSkill(Vector2 targetPosition)
    {
        StartCoroutine(SkillRoutine(targetPosition));
    }


    private IEnumerator SkillRoutine(Vector2 targetPosition)
    {
        _navMeshAgent.enabled = false;
        // 1. 보스를 특정 위치로 이동시키기 (위로 올리기)
        Vector2 upPosition = new Vector2(targetPosition.x, targetPosition.y + UpHeight);
        yield return MoveToPosition(upPosition);

        // 2. 보스가 올라간 동안 Cast Area 프리팹을 표시
        ShowCastArea(targetPosition);

        // 잠시 대기 (보스가 올라간 상태 유지)
        yield return new WaitForSeconds(1f);

        // 3. 보스를 타겟의 위치로 내려찍기
        yield return MoveToPosition(targetPosition);

        // 4. 원형 특정 범위 내 타겟에게 피해
        DealDamage(targetPosition);

        // Cast Area 숨기기
        HideCastArea();

        _navMeshAgent.enabled = true;

        _isSkillRunning = false;
        _skillCompleted = true;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void ShowCastArea(Vector3 position)
    {
        _castAreaObject = PoolManager.Instance.GetGameObject("CircleCastArea");
        _castAreaObject.transform.position = position;
        _castAreaObject.transform.localScale = new Vector2(DamageRadius, DamageRadius);
    }

    private void HideCastArea()
    {
        _castAreaObject.GetComponent<PoolAble>().ReleaseObject();
    }

    private void DealDamage(Vector3 position)
    {
        GameObject effectInstance = PoolManager.Instance.GetGameObject("DownwardStrikeEffect");
        effectInstance.transform.position = position;
        StartCoroutine(DisableEffectAfterDelay(effectInstance, 0.5f));

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, DamageRadius, TargetLayer);
        foreach (var hitCollider in hitColliders)
        {
            // 타겟에게 피해를 입히는 로직 (예: 타겟의 체력을 감소시키기)
            hitCollider.GetComponent<IDamageable>().TakeDamage(50); // 예시로 50의 피해를 줍니다.
        }
    }
    private IEnumerator DisableEffectAfterDelay(GameObject effectInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        effectInstance.GetComponent<PoolAble>().ReleaseObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DamageRadius);
    }
}

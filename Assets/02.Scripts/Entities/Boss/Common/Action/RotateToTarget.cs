using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
public class RotateToTarget : Action
{
    private GameObject _mainSprite;
    private GameObject _target; // 타겟 게임 오브젝트
    public override void OnAwake()
    {
        _target = BossComponents.Instance.Player;
        _mainSprite = BossComponents.Instance.MainSprite;
    }

    public override TaskStatus OnUpdate()
    {
        if (_target == null)
        {
            return TaskStatus.Failure;
        }

        RotateTowardsTarget(_target.transform);
        return TaskStatus.Success;
    }

    private void RotateTowardsTarget(Transform targetTransform)
    {
        Vector2 direction = (targetTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _mainSprite.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Abs(angle) > 90 ? 0 : 180, 0));
    }
}

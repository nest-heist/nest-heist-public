using System;
using UnityEngine;

/// <summary>
/// 투사체에 붙히는 컴포넌트
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector2 targetPos;
    private float speed;
    private Action<Vector2> onHitAction;

    public void Initialize(Vector2 targetPos, float speed, Action<Vector2> onHitAction)
    {
        this.targetPos = targetPos;
        this.speed = speed;
        this.onHitAction = onHitAction;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (targetPos == null)
        {
            gameObject.SetActive(false);
            return;
        }
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPos - currentPosition).normalized;
        // 투사체를 목표를 향해 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // 목표에 도달 시 충돌 처리
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            OnHitTarget();
        }
    }

    private void OnHitTarget()
    {
        // 목표 위치에 광역 효과 적용
        onHitAction?.Invoke(targetPos);

        // 충돌 애니메이션 및 효과 재생
        PlayCollisionEffect();

        // 투사체 제거
        gameObject.SetActive(false);
    }

    private void PlayCollisionEffect()
    {
        // 충돌 시 애니메이션 및 파티클 효과 재생
        // 예: Instantiate(collisionEffectPrefab, transform.position, Quaternion.identity);
    }
}

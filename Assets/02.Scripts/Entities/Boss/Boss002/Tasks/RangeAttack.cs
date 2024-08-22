
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class RangeAttack : Action
{
    public float ProjectileSpeed = 12f; // 프로젝타일 속도
    public float SpeedVariance = 0.4f; // 속도 변동 범위 (10%)

    private int _projectilesLeft;
    private bool _attackStarted = false;
    public LayerMask TargetLayer; // 타겟이 속한 레이어

    public override void OnAwake()
    {
    }
    public override void OnStart()
    {
        _attackStarted = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (!_attackStarted)
        {
            _attackStarted = true;
            _projectilesLeft++;
            SpawnProjectile(BossComponents.Instance.Player.transform.position);
        }

        if (_projectilesLeft <= 0)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void SpawnProjectile(Vector2 targetPos)
    {
        GameObject projectileObject = PoolManager.Instance.GetGameObject("RangeAttackProjectile");
        projectileObject.transform.position = transform.position;
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // 속도 변동 범위 내에서 랜덤 속도 설정
        float randomSpeed = ProjectileSpeed * Random.Range(1f - SpeedVariance, 1f + SpeedVariance);

        // 초기화 및 타겟 설정
        projectile.Initialize(targetPos, randomSpeed, (pos) => { OnProjectileHit(pos, projectile); });
    }

    private void OnProjectileHit(Vector2 hitPosition, Projectile projectile)
    {
        projectile.GetComponent<PoolAble>().ReleaseObject();

        GameObject explosionEffect = PoolManager.Instance.GetGameObject("MeteorHitEffect");
        explosionEffect.transform.position = hitPosition;
        explosionEffect.SetActive(true);
        StartCoroutine(ReleaseExplosionEffectAfterDelay(explosionEffect, 0.5f));

        if(hitPosition == null)
        {
            Debug.LogError("히트포지션이 비어있다.");
        }

        if(projectile == null)
        {
            Debug.LogError("프로젝타일이 비어있다.");
        }

        // 추가: 플레이어에게 데미지 주기
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitPosition, 1f, TargetLayer);
        foreach (Collider2D collider in colliders)
        {
            collider.GetComponent<IDamageable>().TakeDamage(20); // 예시로 50의 피해를 줍니다.
        }

        _projectilesLeft--;
    }

    private IEnumerator ReleaseExplosionEffectAfterDelay(GameObject explosionEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionEffect.GetComponent<PoolAble>().ReleaseObject();
    }
}

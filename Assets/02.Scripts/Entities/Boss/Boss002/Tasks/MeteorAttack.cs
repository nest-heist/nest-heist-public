using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MeteorAttack : Action
{
    private Tilemap _tilemap; // 타일맵을 참조
    public Vector2 SpawnPos; // 스폰 위치
    public int NumberOfProjectiles = 50;
    public float ProjectileSpeed = 12f; // 프로젝타일 속도
    public float SpeedVariance = 0.4f; // 속도 변동 범위 (10%)

    private int _projectilesLeft;
    private bool _attackStarted = false;
    public float ExplosionRadius = 0.5f; // 피해를 주는 반경
    public LayerMask TargetLayer; // 타겟이 속한 레이어

    public override void OnAwake()
    {
        _tilemap = BossComponents.Instance.Map;
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
            _projectilesLeft = NumberOfProjectiles;
            List<Vector2> randomPositions = GetRandomPositionsInTilemap(_tilemap, NumberOfProjectiles);
            foreach (Vector2 position in randomPositions)
            {
                SpawnProjectile(position);
            }
        }

        if (_projectilesLeft <= 0)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private List<Vector2> GetRandomPositionsInTilemap(Tilemap tilemap, int count)
    {
        List<Vector2> randomPositions = new List<Vector2>();
        
        // 타일맵의 바운드 내에서 랜덤 위치 선택
        BoundsInt bounds = tilemap.cellBounds;
        for (int i = 0; i < count; i++)
        {
            Vector3Int randomCellPosition = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            // 셀 위치를 월드 위치로 변환
            Vector3 randomWorldPosition = tilemap.CellToWorld(randomCellPosition);
            randomPositions.Add(randomWorldPosition);
        }

        return randomPositions;
    }

    private void SpawnProjectile(Vector2 targetPos)
    {
        GameObject castAreaObject = PoolManager.Instance.GetGameObject("MeteorCastArea");
        castAreaObject.transform.position = targetPos;

        // 프로젝타일 생성
        GameObject projectileObject = PoolManager.Instance.GetGameObject("Meteor");
        projectileObject.transform.position = SpawnPos;
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // 속도 변동 범위 내에서 랜덤 속도 설정
        float randomSpeed = ProjectileSpeed * Random.Range(1f - SpeedVariance, 1f + SpeedVariance);

        // 초기화 및 타겟 설정
        projectile.Initialize(targetPos, randomSpeed, (pos) => { OnProjectileHit(pos, projectile, castAreaObject); });
    }

    private void OnProjectileHit(Vector2 hitPosition, Projectile projectile, GameObject castAreaObject)
    {
        castAreaObject.GetComponent<PoolAble>().ReleaseObject();
        projectile.GetComponent<PoolAble>().ReleaseObject();

        GameObject explosionEffect = PoolManager.Instance.GetGameObject("MeteorHitEffect");
        explosionEffect.transform.position = hitPosition;
        explosionEffect.SetActive(true);
        StartCoroutine(ReleaseExplosionEffectAfterDelay(explosionEffect, 0.5f));

        // 추가: 플레이어에게 데미지 주기
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitPosition, ExplosionRadius, TargetLayer);
        foreach (Collider2D collider in colliders)
        {
            collider.GetComponent<IDamageable>().TakeDamage(50); // 예시로 50의 피해를 줍니다.
        }

        _projectilesLeft--;
    }

    private IEnumerator ReleaseExplosionEffectAfterDelay(GameObject explosionEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionEffect.GetComponent<PoolAble>().ReleaseObject();
    }
}

using DG.Tweening;
using UnityEngine;

public class ItemDropEffect : PoolAble
{
    private Transform _target;
    private float _isDoneDotween;

    public void SetTarget(Transform target)
    {
        _target = target;
        _isDoneDotween = 0.5f;
        Explosion(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, target.position.y), 1f);
    }


    public void Explosion(Vector2 from, Vector2 to, float explo_range)
    {
        // 이펙트의 초기 위치를 'from' 위치로 설정
        transform.position = from;

        // 새로운 DOTween 시퀀스를 생성
        Sequence sequence = DOTween.Sequence();

        // 첫 번째로, 'from' 위치에서 폭발 범위 내의 랜덤 위치로 오브젝트를 이동
        sequence.Append(transform.DOMove(from + Random.insideUnitCircle * explo_range, 0.25f).SetEase(Ease.OutCubic));

        // 그 다음, 오브젝트를 'to' 위치(현재는 플레이어)로 이동
        sequence.OnUpdate(() =>
            {
                if (_target != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * 15);
                    float distanceToTarget = Vector3.Distance(transform.position, _target.position);
                    
                    if (distanceToTarget <= _isDoneDotween)
                    {
                        AudioManager.Instance.PlaySFX("Coin");
                        ReleaseObject();
                        sequence.Kill();
                    }
                }
            })
             .AppendInterval(3f);

        // 범위 안에 못들어 가더라도 시간이 지나면 게임 오브젝트를 비활성
        sequence.AppendCallback(() => { ReleaseObject(); });
    }
}
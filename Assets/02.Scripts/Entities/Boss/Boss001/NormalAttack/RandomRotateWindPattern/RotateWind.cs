using UnityEngine;

/// <summary>
/// 일정방향으로 회전하는 바람
/// </summary>
public class RotateWind : PoolAble
{
    [SerializeField] public Transform Center;
    public float radius = 0.6f;
    public float speed = 20f;
    public float angle = 0;

    private float _damage;

    public void InitCenter(Transform center)
    {
        Center = center;
    }

    public void InitDamage(int amount)
    {
        _damage = amount;
    }

    private void FixedUpdate()
    {
        angle += speed * Time.fixedDeltaTime;
        transform.position = Center.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage((int)_damage * 2);
        }
    }
}

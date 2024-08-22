using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Wind : PoolAble
{
    public Vector2 dir;
    [SerializeField] public float _windSpeed;

    private float _damage;
    private void Start()
    {
        _windSpeed = 12f;
    }

    public void InitDamage(int amount)
    {
        _damage = amount;   
    }
    private void FixedUpdate()
    {
        transform.position += (Vector3)dir * _windSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage((int)_damage);
        }
    }
}

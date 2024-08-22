using UnityEngine;
using System.Collections;
public class ForceReceiver : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Player _player;

    [Header("KnockBack")]
    public float KnockbackForce = 15f;
    public float KnockbackDuration = 0.2f;
    public WaitForSeconds KnockbackCooldown = new WaitForSeconds(1f);
    private bool _canKnockback = true;

    float _drag = 0.3f;
    Vector2 _impact;
    Vector2 _dampingVelocity;

    public Vector2 Movement => _impact;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }
    private void Update()
    {
        _impact = Vector2.SmoothDamp(_impact, Vector2.zero, ref _dampingVelocity, _drag);
    }

    public void Reset()
    {
        _impact = Vector2.zero;
    }

    public void AddForce(Vector2 force)
    {
        _impact += force;
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (_canKnockback)
        {
            _rb.velocity = Vector2.zero;
            _player._stateMachine.CanMove = false;
            _canKnockback = false;
            StartCoroutine(KnockbackCoroutine(direction));
        }
    }

    public IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        float timer = 0;
        while (timer <= KnockbackDuration)
        {
            _impact = direction * KnockbackForce;
            timer += Time.deltaTime;
            yield return null;
        }
        _rb.velocity = Vector2.zero;
        yield return KnockbackCooldown;
        _canKnockback = true;
        _player._stateMachine.CanMove = true;
    }
}

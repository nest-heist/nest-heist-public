using UnityEngine;
using System.Collections;
public class MonsterForceReceiver : MonoBehaviour
{
    private BaseMonster _monster;

    [Header("KnockBack")]
    public float _knockbackForce = 3f;
    public float KnockbackDuration = 0.2f;
    public WaitForSeconds KnockbackCooldown = new WaitForSeconds(1f);
    private bool _canKnockback = true;



    public void Init()
    {
        _monster = GetComponent<BaseMonster>();
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (_canKnockback)
        {
            _monster.AIHandler.Agent.isStopped = true;
            _monster.AIHandler.Agent.velocity = Vector2.zero;
            _canKnockback = false;
            StartCoroutine(KnockbackCoroutine(direction));
        }
    }

    public IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        float timer = 0;
        while (timer <= KnockbackDuration)
        {
            _monster.AIHandler.Agent.velocity = direction * _knockbackForce;
            timer += Time.deltaTime;
            yield return null;
        }
        _monster.AIHandler.Agent.velocity = Vector2.zero;
        yield return KnockbackCooldown;
        _canKnockback = true;
        _monster.AIHandler.Agent.isStopped = false;
    }
}

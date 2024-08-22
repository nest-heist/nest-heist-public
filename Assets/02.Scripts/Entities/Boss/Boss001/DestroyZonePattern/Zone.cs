using System.Collections;
using UnityEngine;

public class Zone : PoolAble
{
    public Vector3 shrinkScale = new Vector3(0.5f, 0.5f, 0.5f);
    // 줄어드는 시간
    public float shrinkDuration = 0.5f;
    private bool isShrinking = false;

    private Vector3 originalScale;
    private Vector2 StartPos;

    private WaitForSeconds TickTime;

    private MON10001 _boss;
    private Player _player;
    private int damage;

    private void Awake()
    {
        originalScale = transform.localScale;
    }
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<MON10001>();
        SetDamage();
        SetTickTime();
    }


    private void OnEnable()
    {
        StartCoroutine(Damage());
        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        StopCoroutine(Damage());
    }


    private void SetDamage()
    {
        damage = _boss.StatHandler.CurrentStat.Attack / 5;
    }

    private void SetTickTime()
    {
        TickTime = new WaitForSeconds(2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isShrinking)
        {
            StartCoroutine(Shrink());
        }
    }

    private IEnumerator Shrink()
    {
        isShrinking = true;
        Vector3 targetScale = originalScale - shrinkScale;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        originalScale = transform.localScale;
        if (transform.localScale.x <= 0 || transform.localScale.y <= 0 || transform.localScale.z <= 0)
        {
            ReleaseObject();
        }
        isShrinking = false;
    }

    private IEnumerator Damage()
    {
        yield return TickTime;
        _player.Health.TakeDamage(damage);
    }
}

using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthSystem : BaseCondition<PlayerStatHandler>, IDamageable
{
    private ForceReceiver _forceReceiver;
    public override ConditionType Type => ConditionType.Health;

    public event Action<int> OnDamageEvent;
    public event Action<int> OnHealEvent;
    public event Action<float, float> OnHPChangedEvent;
    public UnityEvent OnDeathEvent;

    private float _regenRate = 1f;
    private WaitForSeconds _invincibleRate = new WaitForSeconds(1f);
    private Coroutine _regenCoroutine;
    private Player _player;
    public bool IsDie { get; private set; }

    public GameObject This { get; private set; }

    public string MonsterId { get; set; }

    protected override void Awake()
    {
    }

    protected override void Start()
    {
    }

    public override void Init()
    {
        base.Init();
        _player = GetComponent<Player>();
        This = this.gameObject;
        _forceReceiver = GetComponent<ForceReceiver>();
        //TODO : 임시값 설정 MAX
        //  Max = 100000;
        Max = _statHandler.CurrentStat.MaxHP;
        _current = Max;
        Modify(0);
        IsDie = false;
        if (_statHandler.CurrentStat.HPRegen != 0)
        {
            _regenCoroutine = StartCoroutine(Regen());
        }
    }

    public override bool Modify(int amount)
    {
        base.Modify(amount);

        if (Current <= 0)
        {
            IsDie = true;
            OnDeathEvent?.Invoke();
        }

        if (amount < 0)
        {
            OnDamageEvent?.Invoke(amount);
        }
        else
        {
            OnHealEvent?.Invoke(amount);
        }

        OnHPChangedEvent?.Invoke((float)Current, (float)Max);
        return true;
    }

    private IEnumerator Regen()
    {
        while (true)
        {
            Modify((int)_statHandler.CurrentStat.HPRegen);
            yield return new WaitForSeconds(_regenRate);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_player.IsInvincible) return;

        // 난수 생성기 인스턴스 생성 (간단한 시드 사용)
        System.Random rng = new System.Random();

        // 0부터 99까지의 난수 생성
        int randomValue = rng.Next(100);

        if (_player.CurrentStat.Avoid > randomValue)
        {
            damage = 0;
        }

        if (damage > 0)
        {
            Vector2 dir = _player._stateMachine.IsFacingRight ? Vector2.left : Vector2.right;
            _forceReceiver.ApplyKnockback(dir);
        }
        
        Modify(-damage);
        SetInvincibility();
    }

    public void SetInvincibility()
    {
        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        _player.IsInvincible = true;
        yield return _invincibleRate;
        _player.IsInvincible = false;
    }

    public bool IsDead()
    {
        return IsDie;
    }

    public bool IsPlayer()
    {
        return true;
    }
}
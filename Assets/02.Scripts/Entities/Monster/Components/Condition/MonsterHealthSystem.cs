using BehaviorDesigner.Runtime;
using System;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;


public interface IDamageable
{
    void TakeDamage(int damage);
    bool IsDead();
    public GameObject This { get; }
    bool IsPlayer();
    public string MonsterId { get; }
}
public class MonsterHealthSystem : BaseCondition<MonsterStatHandler>, IDamageable
{
    public override ConditionType Type => ConditionType.Health;

    public event Action<int> OnDamageEvent;
    public event Action<int> OnHealEvent;
    public event Action<float, float> OnHPChangedEvent;
    public UnityEvent OnDeathEvent;

    private float _regenRate = 0.5f;
    private Coroutine _regenCoroutine;

    private BaseMonster _monster;

    public WaitForSeconds _invincibleRate = new WaitForSeconds(1f);
    public bool IsDie { get; private set; }

    public GameObject This { get; private set; }

    public string MonsterId { get; set; } = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }


    public override void Init()
    {
        base.Init();
        This = this.gameObject;
        _monster = GetComponent<BaseMonster>();
        IsDie = false;
        Max = _statHandler.CurrentStat.MaxHP;
        _current = Max;
        Modify(0);
    }

    public override bool Modify(int amount)
    {
        if (IsDie == true) { return false; }
        base.Modify(amount);
        if (Current <= 0)
        {
            IsDie = true;
            SendDieEventToBT();
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

    public void TakeDamage(int damage)
    {
        if (_monster != null)
        {
            if (_monster.IsInvincible) return;

            _monster.IsRight = Mathf.Abs(_monster.MainSprite.transform.rotation.eulerAngles.y) == 180 ? true : false;
            Vector2 dir = _monster.IsRight ? Vector2.left : Vector2.right;
            _monster.ForceReceiver.ApplyKnockback(dir);

            // 데미지를 방어력으로 감소시키는 부분.
            if (damage > 0)
            {
                // 방어력을 적용하여 데미지를 감소,
                // TODO : 1부분은 추후 Max방어력으로 바꿔야 함

                float defenseEffect = (float)_monster.StatHandler.CurrentStat.Defence / 1000f;

                damage = Mathf.FloorToInt(damage * (1 - defenseEffect));

                // 데미지가 0 이하로 내려가지 않도록 함
                if (damage < 0)
                {
                    damage = 0;
                }
            }
        }

        Modify(-damage);
        SetInvincibility();
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        AudioManager.Instance.PlaySFX("Generic_Hit");
    }

    public void SetInvincibility()
    {
        if (Current > 0)
        {
            StartCoroutine(Invincibility());
        }
    }

    private IEnumerator Invincibility()
    {
        _monster.IsInvincible = true;
        yield return _invincibleRate;
        _monster.IsInvincible = false;
    }

    public bool IsDead()
    {
        return IsDie;
    }

    public float GetHealthPercentage()
    {
        return (float)Current / (float)Max;
    }

    public void SendDieEventToBT()
    {
        if (gameObject.TryGetComponent<BehaviorTree>(out BehaviorTree component))
        {
            component.SendEvent("Die");
        }
    }

    public bool IsPlayer()
    {
        return false;
    }
}
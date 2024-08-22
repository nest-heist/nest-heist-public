using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss : MonoBehaviour
{
    public Tilemap Room;
    public MonsterStatHandler StatHandler;
    public BossHealthSystem HealthSystem { get; private set; }
    public Player Player { get; set; }
    private GameObject _target;
    protected virtual void Awake()
    {
        StatHandler = GetComponent<MonsterStatHandler>();
        StatHandler.Init();
        HealthSystem = GetComponent<BossHealthSystem>();
        HealthSystem.Init();
    }

    protected virtual void Start()
    {
        HealthSystem.OnDeathEvent.AddListener(OnDie);
        _target = BossComponents.Instance.Player;
    }

    protected virtual void OnDie()
    {
        Destroy(this.gameObject);
    }

    public void Init(Player player)
    {
        Player = player;
    }

    public void OnAttack()
    {
        if (_target.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage(50);
        }
    }
}

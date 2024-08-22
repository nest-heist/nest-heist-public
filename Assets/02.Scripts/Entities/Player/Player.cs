using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Partner;
    [field: SerializeField] public Transform MainSprite { get; private set; }
    [field: SerializeField] public PlayerAnimeData AnimeData { get; private set; }
    public PlayerInput Input { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody2D Rigid { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    public PlayerStatHandler StatHandler { get; private set; }
    public PlayerHealthSystem Health { get; private set; }
    public PlayerStateMachine _stateMachine;

    public Transform EggPosition;
    public bool CarryingEgg;
    public bool IsInvincible;

    private int _monsterEggLayer = 13;

    public ServerUserGameInfoData UserData;
    public PlayerStat CurrentStat { get; private set; }
    public PlayerStat BaseStat { get; private set; }
    public PlayerStat LevelStat { get; private set; }

    private PlayerAllStatData _playerAllSatData;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Input = GetComponent<PlayerInput>();
        StatHandler = GetComponent<PlayerStatHandler>();
        StatHandler.Init();
        Health = GetComponent<PlayerHealthSystem>();
        Health.Init();

        AnimeData.Initialize();

        _stateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        IsInvincible = false;
        UserData = DataManager.Instance.ServerDataSystem.User;
        _playerAllSatData = DataManager.Instance.ReadOnlyDataSystem.PlayerAllStat[UserData.PlayerId];
        _stateMachine.ChangeState(_stateMachine.IdleState);

        // 기본 스텟 초기화
        BaseStat = _playerAllSatData.BaseStat.ReturnStat().DeepCopy();
        CurrentStat = BaseStat.DeepCopy();

        // 초기 스텟 계산
        UpdateLevelStat();
        GameManager.Instance.UserSystem.PlayerDie = false;
    }

    private void Update()
    {
        _stateMachine.HandleInput();
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.PhysicsUpdate();
    }

    private void OnEnable()
    {
        Health.OnDamageEvent += HandleDamage;
        Health.OnDeathEvent.AddListener(HandleDeath);
    }

    private void OnDisable()
    {
        Health.OnDamageEvent -= HandleDamage;
        Health.OnDeathEvent.RemoveListener(HandleDeath);
    }

    private void HandleDamage(int amount)
    {
        // 무적 상태가 아닐 때만 Hit스테이트로 진입
        if (!IsInvincible)
        {
            _stateMachine.ChangeState(_stateMachine.HitState);
        }
    }

    private void HandleDeath()
    {
        _stateMachine.ChangeState(_stateMachine.DieState);
    }

    public void OnGameOverPanel()
    {
        GameManager.Instance.UserSystem.PlayerDie = true;
        UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonRetryOrExit>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _monsterEggLayer)
        {
            CarryingEgg = true;
        }
    }

    public void UpdateLevelStat()
    {
        LevelStat = _playerAllSatData.LevelStat.ReturnStat().StatCalculator(StatCalculator.CalculateLevelStat, UserData.Level);
        CurrentStat.Add(LevelStat);
    }

    private void WalkAnimationEvent()
    {
        AudioManager.Instance.PlaySFX("Walk");
    }
}

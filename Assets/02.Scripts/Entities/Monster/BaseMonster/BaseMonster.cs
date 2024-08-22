using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [field: SerializeField] public Transform MainSprite { get; set; }
    [field: SerializeField] public MonsterAnimeData AnimeData { get; set; } = new MonsterAnimeData();
    //몬스터 데이터
    public Animator Animator { get; set; }
    public Rigidbody2D Rigidbody { get; set; }
    /*public HealthSystem Health { get; private set; }
    public StaminaSystem Stamina { get; private set; }*/
    //태그로 찾거나 매니저를 통해서 찾아야할것 같다.
    public Player Player { get; set; }
    public MonsterStateMachine StateMachine;
    public BaseMonsterAIHandler AIHandler { get; set; }
    public AttackHandler AttackHandler { get; set; }
    public MonsterStatHandler StatHandler { get; set; }
    public MonsterHealthSystem HealthSystem { get; set; }
    public EffectHandler MontserEffectHandler { get; set; }
    public SkillHandler SkillHandler { get; set; }
    public ActionModeHandler ActionModeHandler { get; set; }
    public MonsterForceReceiver ForceReceiver { get; set; }
    public MonsterUI MonsterUI { get; set; }
    public Attack Attack { get; set; }
    public int Lv { get; set; }
    public bool IsInvincible { get; set; }
    public bool IsReturned { get; set; }
    public bool IsPartner { get; set; }
    public bool IsRight { get; set; }

    protected virtual void Awake() { }
    protected virtual void Start() { }
    public void Init()
    {
        AIHandler.Init();
        AttackHandler.Init();
        SkillHandler.Init();
        ForceReceiver.Init();
        MonsterUI.Init();
        StateMachine = new MonsterStateMachine(this);
        StateMachine.ChangeState(StateMachine.IdleState);
        HealthSystem.OnDamageEvent -= Hit;
        HealthSystem.OnDamageEvent += Hit;
        HealthSystem.OnDeathEvent.RemoveAllListeners();
        HealthSystem.OnDeathEvent.AddListener(Die);
        IsReturned = true;
    }
    public void ComponentInit()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        StatHandler = GetComponent<MonsterStatHandler>();
        HealthSystem = GetComponent<MonsterHealthSystem>();
        AIHandler = GetComponent<BaseMonsterAIHandler>();
        AttackHandler = GetComponent<AttackHandler>();
        MontserEffectHandler = GetComponent<EffectHandler>();
        SkillHandler = GetComponent<SkillHandler>();
        ActionModeHandler = new ActionModeHandler();
        ForceReceiver = GetComponent<MonsterForceReceiver>();
        MonsterUI = GetComponentInChildren<MonsterUI>();
        Attack = GetComponentInChildren<Attack>();
        //TODO : 나중에 수정
        MainSprite = transform.GetChild(0);
        if (TryGetComponent(out Animator animator))
        {
            Animator = animator;
        }
        else
        {
            Animator = MainSprite.GetComponent<Animator>();
        }

        //Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    protected virtual void Update()
    {
        if (StateMachine != null)
        {
            StateMachine.HandleInput();
            StateMachine.Update();
        }
    }
    protected virtual void FixedUpdate()
    {
        if(StateMachine != null)
        {
            StateMachine.PhysicsUpdate();
        }
    }

    public void Die()
    {
        // 파트너 몬스터가 바로 비활성화 된다 -> 죽는 코루틴, 그 뒤 함수들 실행 못한다
        StateMachine.ChangeState(StateMachine.DieState);
        if (IsPartner) return;
        StartCoroutine(DieObject());
    }

    public void Hit(int amount)
    {
        if (HealthSystem.Current <= 0) return;

        MontserEffectHandler.ActiveEffect("Hit", MontserEffectHandler.HitTransform);

        StateMachine.ChangeState(StateMachine.HitState);
    }

    IEnumerator DieObject()
    {
        yield return new WaitForSeconds(1f);
        // Destroy(gameObject);
        transform.position = Vector3.zero;
        MontserEffectHandler.ReleaseDynamicObject();
        SkillHandler.DestroySkillObject();
    }

    private void OnDestroy()
    {
        if (SkillHandler == null) return;
        SkillHandler.DestroySkillObject();
    }
}

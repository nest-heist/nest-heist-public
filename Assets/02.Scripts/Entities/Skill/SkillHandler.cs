using System.Collections;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    private BaseMonster _monster;
    public MonsterSkillInfoData Info;
    private GameObject _skillPrefab;
    private AttackSkill _skillInstance;
    public bool IsCooldown = false;
    private bool _isCastTime = false;
    private float _cooldownEndTime;
    private float _cooldownStartTime;

    // 현재 실행 중인 쿨다운 코루틴을 추적
    private Coroutine _cooldownCoroutine;

    public float RemainingCooldown => IsCooldown ? _cooldownEndTime - Time.time : 0f;

    WaitForSeconds _castTime;

    public int LayerMask;
    public Collider2D Collider2D { get; private set; }


    public void Init()
    {
        Collider2D = GetComponent<Collider2D>();
    }
    public bool GetIsCooldown()
    {
        return IsCooldown;
    }

    public bool GetIsCastTime()
    {
        return _isCastTime;
    }

    public void Init(string skillId)
    {
        Info = DataManager.Instance.ReadOnlyDataSystem.MonsterSkillInfo[skillId];
        _monster = GetComponent<BaseMonster>();

        if (_monster.IsPartner)
        {
            // TODO : 현재 호출순서 검토해본 결과 Init단계에서 플레이어 초기화가 이뤄지지 않음
            // 동적 할당이 아니라, 직접 레이어를 지정, 추후 해결하면 수정해야함
            _castTime = new WaitForSeconds(0);
            LayerMask = (1 << 6) | (1 << 8);
        }

        else
        {
            _castTime = new WaitForSeconds(Info.CastTime);
            LayerMask = 1 << gameObject.layer;
        }

        string prefabPath = $"Prefabs/Skill/{skillId}";
        _skillPrefab = ResourceManager.Instance.Load<GameObject>(prefabPath);
        _skillInstance = Instantiate(_skillPrefab, transform.position, Quaternion.identity).GetComponent<AttackSkill>();
        _skillInstance.Init(Info, LayerMask, _monster);
        //최적화를 위해서 한번만 객체 생성해놓고 반복적으로 사용
    }

    public bool Use(GameObject target)
    {
        if (IsCooldown || _isCastTime)
        {
            return false;
        }

        StartCoroutine(CastSkill(target));
        return true;
    }

    private IEnumerator CastSkill(GameObject target)
    {
        _isCastTime = true;

        // 스킬 시전 시간 동안 대기
        _skillInstance.transform.position = transform.position;
        _skillInstance.ApplyCastArea(target.transform.position);
        yield return _castTime;

        // 몬스터가 죽었는지 확인
        if (_monster.HealthSystem.IsDie)
        {
            _isCastTime = false;
            yield break;
        }
        // 쿨다운 시작
        StartCoroutine(CooldownCoroutine());
        ApplyEffect(target);
    }
    private void ApplyEffect(GameObject target)
    {
        _skillInstance.ApplyEffect(target);
    }

    private IEnumerator CooldownCoroutine()
    {    
        IsCooldown = true;
        _cooldownStartTime = Time.time;
        _cooldownEndTime = _cooldownStartTime + Info.CoolDown;

        if (!_monster.IsPartner)
        {
            yield return new WaitForSeconds(Info.CoolDown);
        }

        else
        {
            while (Time.time < _cooldownEndTime && _monster.IsPartner)
            {
                float remainingCooldown = _cooldownEndTime - Time.time;
                if (remainingCooldown < 0.01f)
                {
                    remainingCooldown = 0;
                }
                SkillEvents.RaiseSkillCooldownUpdate(remainingCooldown);

                if (_monster.HealthSystem.IsDie)
                {
                    IsCooldown = false;
                    _isCastTime = false;
                    yield break;
                }

                yield return null;
            }
        }
        IsCooldown = false;
        _isCastTime = false;
    }

    private void StartRestoreCooldown(float remainingCooldown)
    {
        if (_cooldownCoroutine != null)
        {
            StopCoroutine(_cooldownCoroutine);
        }
        _cooldownCoroutine = StartCoroutine(RestoreCooldown(remainingCooldown));
    }

    private IEnumerator RestoreCooldown(float remainingCooldown)
    {
        IsCooldown = true;

        while (Time.time < _cooldownEndTime)
        {
            remainingCooldown = _cooldownEndTime - Time.time;
            if (remainingCooldown < 0.01f)
            {
                remainingCooldown = 0;
            }
            SkillEvents.RaiseSkillCooldownUpdate(remainingCooldown);
            yield return null;
        }
        IsCooldown = false;
        _isCastTime = false;

        yield return new WaitUntil(() => Time.time >= _cooldownEndTime);
    }

    public void DestroySkillObject()
    {
        if(_skillInstance == null) return;
        Destroy(_skillInstance.gameObject);
    }

    public void SaveCooldownState()
    {
        // 현재 쿨다운 상태와 종료 시점을 저장
        if (IsCooldown)
        {
            _cooldownEndTime = Time.time + (Info.CoolDown - (Time.time - _cooldownStartTime));
        }
    }

    public void LoadCooldownState()
    {
        // 저장된 쿨다운 종료 시점을 바탕으로 쿨다운 상태를 복원
        if (IsCooldown)
        {
            StartRestoreCooldown(_cooldownEndTime - Time.time);
        }
    }
}
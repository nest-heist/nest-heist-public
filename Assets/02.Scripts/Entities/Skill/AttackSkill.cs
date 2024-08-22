using UnityEngine;

public abstract class AttackSkill : MonoBehaviour
{
    [field: SerializeField] public GameObject CastAreaObject { get; set; }
    protected MonsterSkillInfoData _info { get; set; }
    protected LayerMask _targetLayer { get; set; }
    protected Vector2 _targetPos { get; set; }
    protected BaseMonster _monster { get; set; }

    public virtual void Init(MonsterSkillInfoData info, LayerMask targetLayer, BaseMonster monster)
    {
        _info = info;
        _targetLayer = targetLayer;
        _monster = monster;
    }

    public virtual void ApplyEffect(GameObject target)
    {
        if (target == null)
        {
            CastAreaObject.SetActive(false);
            return;
        }
    }
    public virtual void ApplyCastArea(Vector2 targetPos)
    {
        _targetPos = targetPos;
        SetCastArea(targetPos);
        CastAreaObject.SetActive(true);
    }
    protected abstract void SetCastArea(Vector2 targetPos);

    public string TargetNamae(GameObject target)
    {
        string targetMonsterCloneName = target.name;
        string targetMonster = targetMonsterCloneName.Replace("(Clone)", "").Trim();
        return targetMonster;
    }
}
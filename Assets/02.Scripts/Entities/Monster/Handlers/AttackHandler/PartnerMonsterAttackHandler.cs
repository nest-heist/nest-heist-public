using UnityEngine;

public class PartnerMonsterAttackHandler : AttackHandler
{
    public override GameObject GetTarget()
    {
        //파트너 몬스터가 바라보고있는 적 반환
        if (Monster.AIHandler.EnemyTargetTransform == null) return null;
        return Monster.AIHandler.EnemyTargetTransform?.gameObject;
    }

    public override void OnDamaging()
    {
        base.OnDamaging();
    }
}

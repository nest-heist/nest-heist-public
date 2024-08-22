using UnityEngine;

public class EnemyMonsterAttackHandler : AttackHandler
{
    public override GameObject GetTarget()
    {
        if (Monster.AIHandler.EnemyTargetTransform == null) return null;
        return Monster.AIHandler.EnemyTargetTransform?.gameObject;
    }

    public override void OnDamaging()
    {
        base.OnDamaging();
    }
}
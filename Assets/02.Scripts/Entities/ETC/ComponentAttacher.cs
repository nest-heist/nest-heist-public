using UnityEngine;
using UnityEngine.AI;

public static class ComponentAttacher
{
    public static void AttachParnterComponent(GameObject obj)
    {
        ChangeLayerRecursively(obj, LayerMask.NameToLayer(Define.PartnerLayer));
        obj.AddComponent<PartnerMonsterAttackHandler>();
        obj.AddComponent<PartnerMonsterAIHandler>();
        obj.AddComponent<PartnerMonster>();
        AttachNavmeshAgent(obj);
    }

    public static void AttachEnemyComponent(GameObject obj)
    {
        ChangeLayerRecursively(obj, LayerMask.NameToLayer(Define.EnemyLayer));

        if (obj.GetComponent<EnemyMonsterAttackHandler>() != null) return;
        obj.AddComponent<EnemyMonsterAttackHandler>();
        obj.AddComponent<EnemyMontserAIHandler>();
        obj.AddComponent<EnemyMonster>();
        obj.AddComponent<MonsterDeathDropChaser>();
        AttachNavmeshAgent(obj);
    }

    private static void AttachNavmeshAgent(GameObject obj)
    {
        if (obj.GetComponent<NavMeshAgent>() != null) return;
        NavMeshAgent agent = obj.AddComponent<NavMeshAgent>();
        agent.speed = 2;
        agent.angularSpeed = 120;
        agent.acceleration = 20;
        agent.stoppingDistance = 1;
        agent.autoBraking = true;

        agent.radius = 0.2f;
        agent.height = 2f;
    }
    private static void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }
}
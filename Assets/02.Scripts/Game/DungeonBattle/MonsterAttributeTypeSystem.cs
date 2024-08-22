using System.Collections.Generic;
using static Define;

/// <summary>
/// 몬스터의 속성과 관련된 시스템을 다루는 클래스
/// 상성의 여부, 데미지의 증감 등을 여기서 결정한다
/// 물(0),전기(1),불(2),얼음(3),드래곤(4),어둠(5),노말(6),땅(7),풀(8)
/// </summary>
public class MonsterAttributeSystem
{
    private Dictionary<AttributeType, List<AttributeRelation>> relations = new Dictionary<AttributeType, List<AttributeRelation>>
    {
        { AttributeType.Water, new List<AttributeRelation> { new AttributeRelation(AttributeType.Fire, 2.0f), new AttributeRelation(AttributeType.Electric, 0.5f) } },
        { AttributeType.Electric, new List<AttributeRelation> { new AttributeRelation(AttributeType.Water, 2.0f), new AttributeRelation(AttributeType.Ground, 0.5f) } },
        { AttributeType.Fire, new List<AttributeRelation> { new AttributeRelation(AttributeType.Grass, 2.0f), new AttributeRelation(AttributeType.Water, 0.5f) } },
        { AttributeType.Ice, new List<AttributeRelation> { new AttributeRelation(AttributeType.Dragon, 2.0f), new AttributeRelation(AttributeType.Fire, 0.5f) } },
        { AttributeType.Dragon, new List<AttributeRelation> { new AttributeRelation(AttributeType.Dark, 2.0f), new AttributeRelation(AttributeType.Ice, 0.5f) } },
        { AttributeType.Dark, new List<AttributeRelation> { new AttributeRelation(AttributeType.Normal, 2.0f), new AttributeRelation(AttributeType.Dragon, 0.5f) } },
        { AttributeType.Ground, new List<AttributeRelation> { new AttributeRelation(AttributeType.Electric, 2.0f), new AttributeRelation(AttributeType.Grass, 0.5f) } },
        { AttributeType.Grass, new List<AttributeRelation> { new AttributeRelation(AttributeType.Ground, 2.0f), new AttributeRelation(AttributeType.Fire, 0.5f) } }
    };

    public float CalculateDamageMultiplier(MonsterInfoData attacker, MonsterInfoData defender)
    {
        float multiplier = 1.0f;

        foreach (var attackerAttr in attacker.AttributeTypeList)
        {
            float maxMultiplier = 1.0f;
            foreach (var defenderAttr in defender.AttributeTypeList)
            {
                float currentMultiplier = GetMultiplier(attackerAttr, defenderAttr);
                if (currentMultiplier > maxMultiplier)
                {
                    maxMultiplier = currentMultiplier;
                }
            }
            multiplier *= maxMultiplier;
        }

        return multiplier;
    }

    public float SkillAttributeTypeMulitiplier(MonsterSkillInfoData attackerSkill, MonsterInfoData defender)
    {
        float multiplier = 1.0f;

        float maxMultiplier = 1.0f;

        foreach (var defenderAttr in defender.AttributeTypeList)
        {
            float currentMultiplier = GetMultiplier(attackerSkill.AttributeType, defenderAttr);
            if(currentMultiplier > maxMultiplier)
            {
                maxMultiplier = currentMultiplier;
            }

            multiplier *= maxMultiplier;
        }

        return multiplier;
    }

    public float GetMultiplier(AttributeType attacker, AttributeType defender)
    {
        if (relations.TryGetValue(attacker, out var relationList))
        {
            foreach (var relation in relationList)
            {
                if (relation.TargetType == defender)
                {
                    return relation.Multiplier;
                }
            }
        }
        return 1.0f;
    }
}

public class AttributeRelation
{
    public AttributeType TargetType { get; }
    public float Multiplier { get; }

    public AttributeRelation(AttributeType targetType, float multiplier)
    {
        TargetType = targetType;
        Multiplier = multiplier;
    }
}
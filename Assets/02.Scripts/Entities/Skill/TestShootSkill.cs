using UnityEngine;

public class TestShootSkill : MonoBehaviour
{
    public GameObject Target;
    private SkillHandler test;

    void Start()
    {
        MonsterStat testStat = new MonsterStat { Attack = 10 };
        GetComponent<MonsterStatHandler>().SetBaseStat(testStat);
        GetComponent<MonsterStatHandler>().UpdateStat();
        test = GetComponent<SkillHandler>();
        test.Init("MSK00011");
        test.LayerMask = Target.layer;
    }

    void Update()
    {
        if (!test.GetIsCooldown() && !test.GetIsCastTime())
        {
            test.Use(Target);
        }
    }
}
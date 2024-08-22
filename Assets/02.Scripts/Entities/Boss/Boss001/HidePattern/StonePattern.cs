using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using System.Collections.Generic;

[TaskCategory("Boss001")]
public class StonePattern : Action
{
    public GameObject Stone;
    public int MaxStoneNum;
    public int CurStoneNum;

    public bool HasExcuted;

    public SharedMON10001 Boss;

    [Header("Start & End Pos")]
    public SharedVector2 StartPos;
    public SharedVector2 EndPos;

    Vector2 TempVector2 = Vector2.zero;


    public override void OnStart()
    {
        CurStoneNum = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if(HasExcuted)
        {
            return TaskStatus.Failure;
        }
        if (CurStoneNum < MaxStoneNum)
        {
            CreateStone();
            CurStoneNum++;
            return TaskStatus.Running;
        }
        else if (CurStoneNum == MaxStoneNum)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        for (int i = 0; i < Boss.Value.Stones.Count; i++)
        {
            Boss.Value.Stones[i].Collider.enabled = false;
        }
        HasExcuted = true;
        CurStoneNum = 0;
    }

    private void CreateStone()
    {
        Stone stone = PoolManager.Instance.GetGameObject("Rock").GetComponent<Stone>();

        TempVector2.x = Random.Range(Boss.Value.TopLeft.x + 1, Boss.Value.TopRight.x);
        TempVector2.y = Boss.Value.TopLeft.y + 3f;
        stone.transform.position = TempVector2;
        //stone.StartPos.Value = StartPos.Value;

        TempVector2.y = Random.Range(Boss.Value.BottomLeft.y + 1, Boss.Value.TopLeft.y - 1);
        EndPos.Value = TempVector2;
        stone.EndPos.Value = this.EndPos.Value;

        Boss.Value.Stones.Add(stone);
    }
}

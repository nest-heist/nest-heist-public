using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss0001")]
public class CreateZonePattern : Action
{
    [Header("Object")]
    public SharedMON10001 MON10001;
    public Player Player;
    [Header("Count")]
    public int MAX;

    private Vector2 StartPos;

    private string _goName = "Zone";
    public override void OnAwake()
    {
        Player = BossBattleManager.Instance.Player.GetComponent<Player>();
    }

    public override void OnStart()
    {
        CreateZone();
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
    private void CreateZone()
    {
        for (int i = 0; i < MAX; ++i)
        {
            Zone zone = PoolManager.Instance.GetGameObject(_goName).GetComponent<Zone>();
            StartPos.x = Random.Range(MON10001.Value.TopLeft.x + 1, MON10001.Value.TopRight.x);
            StartPos.y = Random.Range(MON10001.Value.BottomLeft.y + 1, MON10001.Value.TopLeft.y - 1);
            zone.gameObject.transform.position = StartPos;
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

public class Teleport : MonoBehaviour
{
    private LayerMask _playerMask;
    private Teleport _linkTeleport;
    public bool IsActive = true;

    private void Awake()
    {
        _playerMask = LayerMask.GetMask("Player");
    }

    public void Init(Teleport linkTeleport)
    {
        _linkTeleport = linkTeleport;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActive && (1 << other.gameObject.layer & _playerMask) != 0)
        {
            _linkTeleport.IsActive = false;
            other.gameObject.transform.position = _linkTeleport.transform.position;

            GameObject partner = other.gameObject.GetComponent<Player>().Partner;
            if (partner != null)
            {
                NavMeshAgent agent = partner.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false; 
                }

                partner.transform.position = _linkTeleport.transform.position;

                if (agent != null)
                {
                    agent.enabled = true; 
                }
            }

            if (DungeonBattleManager.Instance.PhaseCount == 2)
            {
                // 현재 플레이어의 위치를 기반으로 방을 찾는다.
                RectInt? currentRoom = DungeonBattleManager.Instance.MapSystem.MapGenerator.GetRoomFromPosition(_linkTeleport.transform.position);
                if (currentRoom.HasValue)
                {
                    DungeonBattleManager.Instance.SpawnSystem.SpawnMonstersInRoomWithEgg(currentRoom.Value, DungeonBattleManager.Instance.InfoData.MonsterSpawnNum / 2);
                }
            }

            AudioManager.Instance.PlaySFX("Portal");
            EventManager.Instance.Invoke(EventType.OnEnterPortal, this);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer & _playerMask) != 0)
        {
            IsActive = true;
        }
    }
}

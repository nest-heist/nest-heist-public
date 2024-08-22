using UnityEngine;

/// <summary>
/// 몬스터가 생성 되면서 Add컴포넌트로 부착될 컴포넌트, 몬스터의 죽음을 추적하고 드랍아이템을 설정한다.
/// </summary>
public class MonsterDeathDropChaser : MonoBehaviour
{
    private MonsterHealthSystem _healthSystem;
    private DungeonDropSystem _dropManager;
    private string _dropMonsterId;

    public void Initialize(DungeonDropSystem dropManager, string id)
    {
        _dropManager = dropManager;
        _dropMonsterId = id;
        _healthSystem = GetComponent<MonsterHealthSystem>();

        if (_healthSystem != null)
        {
            _healthSystem.OnDeathEvent.AddListener(OnMonsterItemDrop);
        }
    }

    private void OnDestroy()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnDeathEvent.RemoveListener(OnMonsterItemDrop);
        }
    }


    private void OnMonsterItemDrop()
    {
        if (_dropManager != null)
        {
            Vector2 dropPosition = transform.position;
            _dropManager.AddDropItemsToInventory(_dropMonsterId, dropPosition);
        }

        EventManager.Instance.Invoke(EventType.OnMonsterDead, this);
    }

}

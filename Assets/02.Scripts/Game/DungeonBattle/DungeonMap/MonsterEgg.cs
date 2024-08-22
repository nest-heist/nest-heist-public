using System.Collections;
using UnityEngine;

public class MonsterEgg : MonoBehaviour
{
    private string _eggId;
    private Player _player;
    private bool _firstContact;
    private Transform _eggPosition;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private int playerLayer = 6;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _firstContact = false;
    }

    public void SetEggId(string id)
    {
        _eggId = id;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == playerLayer && !_firstContact)
        {
            if (_player == null)
            {
                _player = other.GetComponent<Player>();
            }

            _firstContact = true;
            _eggPosition = _player.EggPosition;
            DungeonBattleManager.Instance.CarryEgg = true;

            // 알 주위에 스폰할 몬스터 수 설정 및 스폰
            int numberOfMonsters = DungeonBattleManager.Instance.InfoData.MonsterSpawnNum;

            DungeonBattleManager.Instance.GetEggIdSet = _eggId;
            DungeonBattleManager.Instance.PhaseSystem.ChangePhase("PhaseTwo");

            StartCoroutine(CarryingEggOnPlayer());
        }

        else if (other.CompareTag("Player"))
        {
            DungeonBattleManager.Instance.CarryEgg = true;
            DungeonBattleManager.Instance.GetEggIdSet = _eggId;
            StartCoroutine(CarryingEggOnPlayer());
        }
    }

    private IEnumerator CarryingEggOnPlayer()
    {
        yield return new WaitForEndOfFrame();

        transform.SetParent(_eggPosition);
        transform.localPosition = Vector3.zero;

        while (_player.CarryingEgg)
        {
            DungeonBattleManager.Instance.CarryEgg = false;
            yield return null;
        }

        DungeonBattleManager.Instance.GetEggIdSet = null;
        this.transform.position = transform.parent.TransformPoint(new Vector3((float)0, (float)-0.33, 0));
        transform.SetParent(null);
        StartCoroutine(NotColliderOnPlayer());
    }

    private IEnumerator NotColliderOnPlayer()
    {
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(2f);
        _collider.enabled = true;
        _spriteRenderer.enabled = true;
    }
}
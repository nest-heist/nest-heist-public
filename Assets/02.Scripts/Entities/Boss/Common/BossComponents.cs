using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class BossComponents : MonoBehaviour
{
    public static BossComponents Instance { get; private set; }
    public Rigidbody2D Body { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public GameObject Player { get; private set; }
    [field: SerializeField] public Tilemap Map { get; private set; }
    [field: SerializeField] public GameObject MainSprite { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Body = GetComponent<Rigidbody2D>();
        Animator = gameObject.GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;
        Player = BossBattleManager.Instance.Player;
    }
}
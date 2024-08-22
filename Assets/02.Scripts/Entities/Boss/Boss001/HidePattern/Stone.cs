using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEditor;
using UnityEngine;

public class Stone : PoolAble
{
    [SerializeField] private GameObject _gizmo;
    private Rigidbody2D _rb;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private LayerMask _partnerLayerMask;

    [SerializeField] private float _speed;
    [SerializeField] private float _detectRange;
    [SerializeField] public CapsuleCollider2D Collider;
    public SharedVector2 StartPos;
    public SharedVector2 EndPos;

    public MON10001 Boss;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        _detectRange = 2f;
    }

    private void OnEnable()
    {
        transform.position = StartPos.Value;
        _gizmo.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, EndPos.Value) >= 0.1f)
        {
            _rb.velocity = Vector2.down * _speed;
            _gizmo.transform.position = EndPos.Value;
        }
        else
        {
            _rb.velocity = Vector2.zero;
            _gizmo.SetActive(false);
            return;
        }
    }

    //바위 뒤에 존재하는지 확인
    public bool CheckPlayerBehind()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + Vector2.left, _detectRange, _playerLayerMask);
        if (colliders.Length > 0)
        {
            return true;
        }
        {
            return false;
        }
    }

    public bool CheckPartnerBehind()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + Vector2.left, _detectRange, _partnerLayerMask);
        if (colliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere((Vector2)transform.position + Vector2.left, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable component))
        {
            component.TakeDamage(100);
        }
    }
}

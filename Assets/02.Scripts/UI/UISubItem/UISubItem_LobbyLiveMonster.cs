using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UISubItem_LobbyLiveMonster : UISubItem
{
    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private Camera _renderCamera;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private Image _AddImage;
    private GameObject _characterInstance;

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayIdleAnimation(_characterInstance);
    }
    public void Clear()
    {
        _rawImage.gameObject.SetActive(false);
        _AddImage?.gameObject.SetActive(true);
    }

    public void Initialize(string monsterId)
    {
        foreach (Transform child in _spawnPoint.transform)
        {
            Destroy(child.gameObject);
        }
        RenderTexture renderTexture = new RenderTexture(500, 500, 16);
        _renderCamera.targetTexture = renderTexture;
        _rawImage.texture = renderTexture;
        _characterInstance = Instantiate(ResourceManager.Instance.Load<GameObject>($"Prefabs/Entities/Monster/{monsterId}"), _spawnPoint.transform);
        _characterInstance.transform.localPosition = Vector2.zero;
        _rawImage.gameObject.SetActive(true);
        _AddImage?.gameObject.SetActive(false);

        RemoveUnnecessaryComponents(_characterInstance);
        RemoveUnnecessaryChildObjects(_characterInstance);
        PlayIdleAnimation(_characterInstance);
    }
    void RemoveUnnecessaryComponents(GameObject obj)
    {
        // 필요한 컴포넌트 외의 모든 컴포넌트 제거
        Destroy(obj.GetComponent<NavMeshAgent>());
        Destroy(obj.GetComponent<CapsuleCollider2D>());
        Destroy(obj.GetComponent<MonsterStatHandler>());
        Destroy(obj.GetComponent<MonsterHealthSystem>());
        Destroy(obj.GetComponent<EffectHandler>());
        Destroy(obj.GetComponent<SkillHandler>());
    }
    void RemoveUnnecessaryChildObjects(GameObject obj)
    {
        // MainSprite를 제외한 모든 자식 오브젝트 파괴
        foreach (Transform child in obj.transform)
        {
            if (child.name != "MainSprite")
            {
                Destroy(child.gameObject);
            }
        }
    }

    void PlayIdleAnimation(GameObject obj)
    {
        if (obj != null)
        {
            Animator animator;
            if (!obj.TryGetComponent(out animator))
            {
                animator = obj.GetComponentInChildren<Animator>();
            }
            else
            {
                animator.speed = 0.5f;
            }
            animator.SetBool("IsIdle", true);
        }
    }
}
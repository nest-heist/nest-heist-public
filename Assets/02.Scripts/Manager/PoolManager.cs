using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    [System.Serializable]
    public class ObjectInfo
    {
        // 오브젝트 이름
        public string objectName;
        // 오브젝트 풀에서 관리할 오브젝트
        public GameObject perfab;
    }
    // 오브젝트풀 매니저 준비 완료표시
    public bool IsReady { get; private set; }

    [SerializeField]
    private ObjectInfo[] _objectInfos = null;

    // 생성할 오브젝트의 key값지정을 위한 변수
    private string _objectName;

    // 오브젝트풀들을 관리할 딕셔너리
    private Dictionary<string, IObjectPool<GameObject>> _ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

    // 오브젝트풀에서 오브젝트를 새로 생성할때 사용할 딕셔너리
    private Dictionary<string, GameObject> _goDic = new Dictionary<string, GameObject>();

    private Dictionary<string, Transform> _parentObjects = new Dictionary<string, Transform>();

    // 동적 할당된 오브젝트 풀 관리
    private Dictionary<string, IObjectPool<GameObject>> _dynamicObjectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();
    private Dictionary<string, GameObject> _dynamicGoDic = new Dictionary<string, GameObject>();
    private Dictionary<string, Transform> _dynamicParentObjects = new Dictionary<string, Transform>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        IsReady = false;

        foreach (var objectInfo in _objectInfos)
        {
            if (_goDic.ContainsKey(objectInfo.objectName))
                continue;

            _goDic.Add(objectInfo.objectName, objectInfo.perfab);

            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                1, // 초기 풀 크기를 1로 설정
                int.MaxValue // 최대 풀 크기 무제한
            );

            _ojbectPoolDic.Add(objectInfo.objectName, pool);

            // 상위 오브젝트 생성
            GameObject parentObject = new GameObject(objectInfo.objectName + "_Parent");
            _parentObjects.Add(objectInfo.objectName, parentObject.transform);

            // 초기 1개의 오브젝트만 미리 생성
            _objectName = objectInfo.objectName;
            PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
            poolAbleGo.Pool.Release(poolAbleGo.gameObject);
        }
        IsReady = true;
    }


    // 생성
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(_goDic[_objectName]);
        poolGo.GetComponent<PoolAble>().Pool = _ojbectPoolDic[_objectName];

        // 부모 오브젝트 설정
        if (_parentObjects.ContainsKey(_objectName))
        {
            poolGo.transform.SetParent(_parentObjects[_objectName]);
        }

        return poolGo;
    }

    // 사용
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGameObject(string goName)
    {
        _objectName = goName;

        if (!_goDic.ContainsKey(goName))
        {
            Debug.LogError($"{goName} Object Does not Exist in PoolManager");
            return null;
        }

        IObjectPool<GameObject> pool = _ojbectPoolDic[goName];

        GameObject pooledObject;
        try
        {
            pooledObject = pool.Get();
        }
        catch
        {
            // 풀에서 오브젝트를 얻을 수 없으면 새로운 오브젝트를 생성하여 풀에 추가
            pooledObject = CreatePooledItem();
            pool.Release(pooledObject);
            pooledObject = pool.Get(); // 다시 가져오기
        }

        return pooledObject;
    }

    // 동적 할당된 오브젝트 가져오기
    public GameObject GetDynamicGameObject(string goName)
    {
        if (!_dynamicGoDic.ContainsKey(goName))
        {
            Debug.LogError($"{goName} Object Does not Exist in PoolManager's dynamic pool.");
            return null;
        }

        IObjectPool<GameObject> pool = _dynamicObjectPoolDic[goName];

        return pool.Get();
    }

    // 동적 오브젝트 풀에 추가하는 메서드
    public void AddDynamicObjectToPool(string objectName, GameObject prefab)
    {
        if (_dynamicGoDic.ContainsKey(objectName))
        {
            Debug.LogWarning($"{objectName} already exists in the dynamic pool.");
            return;
        }

        _dynamicGoDic.Add(objectName, prefab);

        IObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            () => CreateDynamicPooledItem(objectName),
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            1,
            int.MaxValue
        );

        _dynamicObjectPoolDic.Add(objectName, pool);

        // 상위 오브젝트 생성
        GameObject parentObject = new GameObject(objectName + "_DynamicParent");
        _dynamicParentObjects.Add(objectName, parentObject.transform);

        // 디버그 로그 출력
        Debug.LogError($"성공적으로 동적 풀에 오브젝트 추가: {objectName}");
    }

    // 동적 오브젝트 생성 메서드
    private GameObject CreateDynamicPooledItem(string objectName)
    {
        if (!_dynamicObjectPoolDic.ContainsKey(objectName))
        {
            Debug.LogError($"동적 풀에 {objectName} 키가 없음");
            return null;
        }

        GameObject poolGo = Instantiate(_dynamicGoDic[objectName]);
        var poolAble = poolGo.GetComponent<PoolAble>();
        poolAble.DynamicPool = _dynamicObjectPoolDic[objectName];

        if (_dynamicParentObjects.ContainsKey(objectName))
        {
            poolGo.transform.SetParent(_dynamicParentObjects[objectName]);
        }

        return poolGo;
    }
}
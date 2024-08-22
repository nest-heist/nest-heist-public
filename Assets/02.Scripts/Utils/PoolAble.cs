using System.CodeDom;
using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }
    public IObjectPool<GameObject> DynamicPool { get; set; }

    public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }

    public void ReleaseDynamicObject()
    {
        DynamicPool.Release(gameObject);
    }
}

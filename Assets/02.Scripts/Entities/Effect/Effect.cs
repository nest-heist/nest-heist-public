using System.Collections;
using UnityEngine;

public class Effect : PoolAble
{
    private WaitForSeconds _effectTime = new WaitForSeconds(1f);

    private void OnEnable()
    {
        StartCoroutine(ReturnObject());
    }

    private IEnumerator ReturnObject()
    {
        yield return _effectTime;
        ReleaseObject();
    }
}

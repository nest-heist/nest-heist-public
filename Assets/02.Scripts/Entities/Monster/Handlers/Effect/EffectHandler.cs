using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 별로 이펙트를 나타낼 위치와, 공통 이펙트를 정리하는 컴포넌트
/// 이펙트를 불러내고 싶을 때 해당 컴포넌트를 참조해서 원하는 위치를 불러내면 된다.
/// ex) 공격, 피격, 걷기 등등
/// </summary>
public class EffectHandler : PoolAble
{
    [Header("Effect Transform")]
    public Transform BaseAttackTrasform;
    public Transform HitTransform;

    [Header("Effect Obejct")]
    public GameObject BaseAttackEffect;
    public GameObject HitEffect;

    public GameObject BaseAttackEffectObj;
    public GameObject HitEffectObj;

    [Header("Text Transform")]
    public Transform DamageTextTransform;

    private WaitForSeconds _effectTime;



    public void ActiveEffect(string effectName, Transform _trasnform)
    {
        GameObject go = PoolManager.Instance.GetGameObject(effectName);
        go.transform.position = _trasnform.position;
    }
}

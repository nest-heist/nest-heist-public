using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 UI의 부모 클래스
/// 특별한 기능은 없지만 나중에 바인딩이나 UIManager에서 사용할 수 있도록 만들어놓음
/// </summary>
public class UICommon : MonoBehaviour
{
    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnEnable()
    {
    }

    protected void OnUIClickSound()
    {
        AudioManager.Instance.PlaySFX("UIClick");
    }

    protected virtual void OnDestory()
    {
    }
}

using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Action OnDamagingEvent;

    public void OnDamaging()
    {
        OnDamagingEvent?.Invoke();
    }
}

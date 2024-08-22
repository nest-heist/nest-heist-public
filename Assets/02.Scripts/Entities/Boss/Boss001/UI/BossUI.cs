using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private Transform _damageTransform;
    private BossHealthSystem _healthSystem;

    private void OnEnable()
    {
        _healthSystem = GetComponentInParent<BossHealthSystem>();
        _healthSystem.OnDamageEvent += PopUpDamageText;
    }

    private void PopUpDamageText(int amount)
    {
        _damageText.text = amount.ToString();
        TextMeshProUGUI obj = Instantiate(_damageText, _damageTransform);
        Destroy(obj, 2f);
    }
}

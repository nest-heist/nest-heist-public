using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarMax;
    [SerializeField] private Image _healthBarCur;

    [SerializeField] private MonsterHealthSystem _healthSystem;

    private void Start()
    {
        InitialLized();
    }

    public void Init()
    {
        InitialLized();
        UpdateBarUI(0);
    }

    private void UpdateBarUI(int amount)
    {
        _healthBarCur.fillAmount = (float)_healthSystem.Current / (float)_healthSystem.Max;
    }

    private void InitialLized()
    {
        _healthSystem = GetComponentInParent<MonsterHealthSystem>();
        _healthSystem.OnDamageEvent -= UpdateBarUI;
        _healthSystem.OnDamageEvent += UpdateBarUI;
    }
}

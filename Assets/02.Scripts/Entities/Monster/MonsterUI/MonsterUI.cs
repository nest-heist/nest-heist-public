using TMPro;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    private BaseMonster _monster;
    [SerializeField] private GameObject _enemyUI;
    [SerializeField] private GameObject _partnerUI;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] public Transform _damageTransform;
    public HealthBar HealthBar;

    //프리팹으로 만들어놓고 몬스터 타입에 따라 붙히는것도 생각해보기
    //현재는 SetActive로 작동함
    public void Init()
    {
        _monster = GetComponentInParent<BaseMonster>();
        SetPartnerMonster();
        SetEnemyMonster();
        _monster.HealthSystem.OnDamageEvent -= PopUpDamageText;
        _monster.HealthSystem.OnDamageEvent += PopUpDamageText;
    }

    private void SetPartnerMonster()
    {
        if (_monster.GetType() == typeof(PartnerMonster))
        {
            _partnerUI.SetActive(true);
        }
    }

    private void SetEnemyMonster()
    {
        if (_monster.GetType() == typeof(EnemyMonster))
        {
            _enemyUI.SetActive(true);
        }
    }

    private void PopUpDamageText(int amount)
    {
        _damageText.text = amount.ToString();
        TextMeshProUGUI obj = Instantiate(_damageText, _damageTransform);
        Destroy(obj, 2f);
    }
}

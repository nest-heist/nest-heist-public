using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;

    private BaseMonster _monster;

    private void Start()
    {
        _monster = GetComponentInParent<BaseMonster>();
        _levelText.text = $"Lv {_monster.Lv}";
    }
}

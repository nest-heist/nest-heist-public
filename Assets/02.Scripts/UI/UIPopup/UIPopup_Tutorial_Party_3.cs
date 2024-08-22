using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Tutorial_Party_3 : UIPopup
{
    [Header("Button")]
    public Button Button1;

    int count = 0;

    UIPopup_PartySetScreen _ui;

    Button bt1;
    Button bt2;

    protected override void Start()
    {
        base.Start();

        Button1.onClick.AddListener(() => { OnClick(); });

        UIPopup_PartySetScreen _ui = GameObject.Find("UIPopup_PartySetScreen").GetComponent<UIPopup_PartySetScreen>();

        // 1
        GameObject right = _ui.Right.gameObject;
        GameObject monster = right.transform.GetChild(0).gameObject;
        UISubItem_MonsterButton monsterbt = monster.GetComponent<UISubItem_MonsterButton>();
        bt1 = monsterbt.GetComponent<Button>();

        // 2
        bt2 = _ui.UpdatePartyButton;

    }

    private void OnClick()
    {
        count++;

        if (count == 1)
        {
            bt1.onClick.Invoke();
        }

        if (count == 2)
        {
            UIManager.Instance.DestoryUIPopup(this);

            bt2.onClick.Invoke();

            if(_ui != null && _ui.gameObject != null)
            {
                Destroy(_ui.gameObject);
            }

            UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial_Party_4>();
        }
    }
}
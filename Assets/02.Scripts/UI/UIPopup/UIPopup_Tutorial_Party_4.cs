using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Tutorial_Party_4 : UIPopup
{
    [Header("Button")]
    public Button NextButton;

    protected override void Start()
    {
        base.Start();

        UIPopup_ShowPartyStateScreen ui = GameObject.Find("UIPopup_ShowPartyStateScreen").GetComponent<UIPopup_ShowPartyStateScreen>();
        Button bt = ui.OnExitBtn;

        NextButton.onClick.AddListener(() => { OnNext(); bt.onClick.Invoke(); OnNextNext();});
    }

    private void OnNext()
    {
        UIManager.Instance.DestoryUIPopup(this);
    }

    private void OnNextNext()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial_Party_5>();
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Tutorial_Party_1 : UIPopup
{
    [Header("Button")]
    public Button NextButton;

    protected override void Start()
    {
        base.Start();

        UIScene_LobbyScene ui = GameObject.Find("UIScene_Lobby").GetComponent<UIScene_LobbyScene>();
        Button bt = ui.GotoPartySetScreenButton;

        NextButton.onClick.AddListener(() => { OnNext(); bt.onClick.Invoke(); DelayedOnNext(); });
    }

    private void OnNext()
    {
        UIManager.Instance.DestoryUIPopup(this);
    }

    private void DelayedOnNext()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial_Party_2>();
    }
}

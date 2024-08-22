using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Tutorial_Party_8 : UIPopup
{
    [Header("Button")]
    public Button NextButton;

    protected override void Start()
    {
        base.Start();

        UIPopup_DungeonEnterCheck ui = GameObject.Find("UIPopup_DungeonEnterCheck").GetComponent<UIPopup_DungeonEnterCheck>();
        Button bt = ui.EnterBtn;


        NextButton.onClick.AddListener(() => { OnNext(); bt.onClick.Invoke(); });
    }

    private void OnNext()
    {
        GameManager.Instance.ClickTutorialSystem.SaveTutorialIndex(8);

        UIManager.Instance.DestoryUIPopup(this);
    }

}

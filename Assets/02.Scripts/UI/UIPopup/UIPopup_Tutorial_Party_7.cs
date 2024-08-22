using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Tutorial_Party_7 : UIPopup
{
    [Header("Button")]
    public Button NextButton;

    protected override void Start()
    {
        base.Start();

        UIPopup_DungeonSelectStage ui = GameObject.Find("UIPopup_DungeonSelectStage").GetComponent<UIPopup_DungeonSelectStage>();
        GameObject go = ui.StageListContainer.GetChild(0).gameObject;

        Button bt = go.GetComponent<Button>();

        NextButton.onClick.AddListener(() => { OnNext(); bt.onClick.Invoke(); DelayedOnNext(); });
    }

    private void OnNext()
    {
        UIManager.Instance.DestoryUIPopup(this);
    }

    private void DelayedOnNext()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial_Party_8>();
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISubItem_Subtask : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI SubtaskDescText;
    public TextMeshProUGUI SubtaskTrackingValueText;

    public UISubItem_Subtask_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UISubItem_Subtask_Presenter(this);
        Presenter.Init();
    }

}

public class UISubItem_Subtask_Presenter
{
    private UISubItem_Subtask _view;

    private Subtask _task;

    public UISubItem_Subtask_Presenter(UISubItem_Subtask view)
    {
        _view = view;
    }

    public void Init()
    {

    }
}

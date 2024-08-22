using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Warning : UIPopup
{
    [Header("Button")]
    public Button CloseButton;

    [Header("Text")]
    public TextMeshProUGUI WarningText;

    public UIPopup_Warning_Presenter _presenter;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIPopup_Warning_Presenter(this);
    }

    public void SetButton(Action OnExit)
    {
        CloseButton.onClick.AddListener(() => { OnExit(); OnUIClickSound(); });
    }

    public void SetText(string warning)
    {
        WarningText.text = warning;
    }
}

public class UIPopup_Warning_Presenter
{
    private UIPopup_Warning _view;

    public UIPopup_Warning_Presenter(UIPopup_Warning view)
    {
        _view = view;

        SetButton();
    }

    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_TitleSpriteText : UIPopup
{
    [Header("Button")]
    public Button CloseButton;

    [Header("Text")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI BasicText;

    [Header("Image")]
    public Image BigImage;

    public UIPopup_TitleSpriteText_Presenter _presenter;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIPopup_TitleSpriteText_Presenter(this);
    }

    public void SetButton(Action OnExit)
    {
        CloseButton.onClick.AddListener(() => { OnExit(); OnUIClickSound(); });
    }

    public void SetText(string title, string basic)
    {
        TitleText.text = title;
        BasicText.text = basic;
    }

    public void SetSprite(Sprite image)
    {
        BigImage.sprite = image;
    }
}

public class UIPopup_TitleSpriteText_Presenter
{
    private UIPopup_TitleSpriteText _view;

    public UIPopup_TitleSpriteText_Presenter(UIPopup_TitleSpriteText view)
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

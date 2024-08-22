using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISubItem_EggInventoryItem : UISubItem
{
    [Header("Image")]
    public Image EggImage;

    [Header("Button")]
    public Button EggButton;

    [Header("Text")]
    public TextMeshProUGUI EggNameText;

    public UISubItem_EggInventoryItem_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UISubItem_EggInventoryItem_Presenter(this);
    }

    public void SetButton(Action OnClickEggButton)
    {
        EggButton.onClick.AddListener(() => { OnClickEggButton(); OnUIClickSound(); });
    }

    public void SetText(string eggName)
    {
        EggNameText.text = eggName;
    }

    public void SetSprite(Sprite sprite)
    {
        EggImage.sprite = sprite;
    }
}

public class UISubItem_EggInventoryItem_Presenter
{
    private UISubItem_EggInventoryItem _view;
    private ServerUserEggData _egg;

    // 부화기 -> 알만 있는 목록에서 사용
    private UIPopup_EggInventory _uiPopup_EggInventory;

    // 인벤토리 -> 인벤토리 + 알 목록에서 사용
    private UIPopup_Inventory _uiPopup_Inventory;

    public UISubItem_EggInventoryItem_Presenter(UISubItem_EggInventoryItem view)
    {
        _view = view;

        SetButton();
    }

    public void SetButton()
    {
        _view.SetButton(OnClickEggButton);
    }

    private void OnClickEggButton()
    {
        if(_uiPopup_EggInventory != null)
        {
            _uiPopup_EggInventory.Presenter.SelectEgg(_egg);
        }
        else if (_uiPopup_Inventory != null)
        {
            _uiPopup_Inventory.Presenter.SelectedItem(_egg);
        }
    }

    /// <summary>
    /// 부화기 -> 알만 있는 목록에서 사용
    /// </summary>
    /// <param name="egg"></param>
    /// <param name="parent"></param>
    public void SetSubItem(ServerUserEggData egg, UIPopup_EggInventory parent)
    {
        _egg = egg;
        _uiPopup_EggInventory = parent;

        string eggName = DataManager.Instance.ReadOnlyDataSystem.EggInfo[egg.EggId].Name;
        Sprite sprite = ResourceManager.Instance.LoadSprite("Icon/EggIcon/eggs", _egg.EggId);

        _view.SetText(eggName);
        _view.SetSprite(sprite);
    }

    /// <summary>
    /// 인벤토리 -> 인벤토리 + 알 목록에서 사용
    /// </summary>
    /// <param name="data"></param>
    /// <param name="view"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetSubItem(ServerUserEggData data, UIPopup_Inventory view)
    {
        _egg = data;
        _uiPopup_Inventory = view;

        string eggName = DataManager.Instance.ReadOnlyDataSystem.EggInfo[data.EggId].Name;
        Sprite sprite = ResourceManager.Instance.LoadSprite("Icon/EggIcon/eggs", data.EggId);
        _view.SetText(eggName);
        _view.SetSprite(sprite);
    }
}

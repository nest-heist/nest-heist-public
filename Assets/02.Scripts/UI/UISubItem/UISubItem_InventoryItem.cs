using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// </summary>
public class UISubItem_InventoryItem : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI ItemCountText;

    [Header("Image")]
    public Image ItemImage;

    [Header("Button")]
    public Button ItemButton;

    public UISubItem_InventoryItem_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UISubItem_InventoryItem_Presenter(this);
    }

    public void SetText(string itemCount)
    {
        ItemCountText.text = itemCount;
    }

    public void SetSprite(Sprite sprite)
    {
        ItemImage.sprite = sprite;
    }

    public void SetButton(Action OnClickItemButton)
    {
        ItemButton.onClick.AddListener(() => OnClickItemButton());
    }
}

public class UISubItem_InventoryItem_Presenter
{
    private UISubItem_InventoryItem _view;

    private UIPopup_Inventory _parent;
    private ServerUserInventoryData _item;

    public UISubItem_InventoryItem_Presenter(UISubItem_InventoryItem view)
    {
        _view = view;

        SetButton();
    }

    public void SetButton()
    {
        _view.SetButton(OnClickItemButton);
    }

    private void OnClickItemButton()
    {
        _parent.Presenter.SelectedItem(_item);
    }

    public void SetSubItem(ServerUserInventoryData data, UIPopup_Inventory view)
    {
        _item = data;
        _parent = view;

        string itemCount = _item.Quantity.ToString();
        Sprite sprite = ResourceManager.Instance.Load<Sprite>($"Icon/ItemIcon/{data.ItemId}");

        _view.SetText(itemCount);
        _view.SetSprite(sprite);
    }
}


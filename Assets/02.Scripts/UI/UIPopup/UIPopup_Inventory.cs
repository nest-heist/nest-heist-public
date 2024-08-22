using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 인벤토리
/// GameManager에 있는 아이템 정보를 UI에 표시
/// ItemId를 가지고 DataManager에서 시트에 있는 데이터 갖고오기
/// 뷰 : 데이터 갖고와서 보여주게 세팅하기
/// 프리젠터 : 로직 처리하기
/// </summary>
public class UIPopup_Inventory : UIPopup
{
    [Header("Button")]
    public Button ExitButton;

    [Header("Text")]
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI ItemDescriptionText;

    [Header("Image")]
    public Image ItemImage;

    [Header("GameObject")]
    public GameObject Right;

    public UIPopup_Inventory_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIPopup_Inventory_Presenter(this);

        Presenter.Init();
    }

    public void SetButton(Action OnGoToLobby)
    {
        ExitButton.onClick.AddListener(() => { OnGoToLobby(); OnUIClickSound(); });
    }

    public void SetText(string itemName, string itemDescription)
    {
        ItemNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
    }

    public void SetSprite(Sprite itemImage)
    {
        ItemImage.sprite = itemImage;
    }
}

public class UIPopup_Inventory_Presenter : Presenter
{
    private UIPopup_Inventory _view;

    private ItemData _seletedItem;

    public UIPopup_Inventory_Presenter(UIPopup_Inventory view)
    {
        _view = view;

        SetButton();
    }

    public void Init()
    {
        DestoryAllISubtems();
        MakeSubItem();

        SelectedItem(GetFirstItem());
    }

    public void SelectedItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError($"UIPopup_Inventory::SelectedItem() item이 널이다");
            return;
        }

        _seletedItem = item;

        SetText();
        SetSprite();
    }

    /// <summary>
    /// 아이템 정보를 오른쪽 UI에 표시
    /// </summary>
    private void MakeSubItem()
    {
        // 아이템 인벤토리 리스트 만들기
        List<ServerUserInventoryData> userInventoryList = DataManager.Instance.ServerDataSystem.UserInventoryList;
        foreach (ServerUserInventoryData data in userInventoryList)
        {
            UISubItem_InventoryItem item = UIManager.Instance.InstanciateSubItem<UISubItem_InventoryItem>(_view.Right.transform);
            item.Presenter.SetSubItem(data, this._view);
        }

        // 알 인벤토리 리스트 만들기 
        List<ServerUserEggData> userEggDataList = DataManager.Instance.ServerDataSystem.UserEggDataList;
        foreach (ServerUserEggData data in userEggDataList)
        {
            UISubItem_EggInventoryItem eggitem = UIManager.Instance.InstanciateSubItem<UISubItem_EggInventoryItem>(_view.Right.transform);
            eggitem.Presenter.SetSubItem(data, this._view);
        }
    }

    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    /// <summary>
    /// 타입 확인해서 띄워줄 텍스트 세팅
    /// </summary>
    private void SetText()
    {
        if(_seletedItem is ServerUserInventoryData inventoryItem)
        {
            ItemInfoData itemInfo = DataManager.Instance.ReadOnlyDataSystem.ItemInfo[inventoryItem.ItemId];
            _view.SetText(itemInfo.Name, itemInfo.Desc);
        }
        else if(_seletedItem is ServerUserEggData eggItem)
        {
            EggInfoData eggInfo = DataManager.Instance.ReadOnlyDataSystem.EggInfo[eggItem.EggId];
            string Desc = $"부화에 필요한 영양제 [{eggInfo.NeedGauge}]\n\n부화에 필요한 시간 [{eggInfo.HatchTime}]분";
            _view.SetText(eggInfo.Name, Desc);
        }
    }

    /// <summary>
    /// 타입 확인해서 띄워줄 이미지 세팅
    /// </summary>
    private void SetSprite()
    {
        if (_seletedItem is ServerUserInventoryData inventoryItem)
        {
            Sprite sprite = ResourceManager.Instance.Load<Sprite>($"Icon/ItemIcon/{inventoryItem.ItemId}");
            _view.SetSprite(sprite);
        }
        else if (_seletedItem is ServerUserEggData eggItem)
        {
            Sprite sprite = ResourceManager.Instance.LoadSprite("Icon/EggIcon/eggs", eggItem.EggId);
            _view.SetSprite(sprite);
        }
    }

    public void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }

    /// <summary>
    /// 디폴트로 첫 번째 아이템 선택하기
    /// </summary>
    /// <returns></returns>
    private ItemData GetFirstItem()
    {
        if (DataManager.Instance.ServerDataSystem.UserInventoryList.Count != 0)
        {
            return DataManager.Instance.ServerDataSystem.UserInventoryList[0];
        }
        if(DataManager.Instance.ServerDataSystem.UserEggDataList.Count != 0)
        {
            return DataManager.Instance.ServerDataSystem.UserEggDataList[0];
        }
        return null;
    }

    private void DestoryAllISubtems()
    {
        foreach (Transform child in _view.Right.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}

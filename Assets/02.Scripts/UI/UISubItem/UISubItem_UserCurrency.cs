using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISubItem_UserCurrency : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI UserGoldText;
    public TextMeshProUGUI UserDiaText;
    public TextMeshProUGUI UserFatigueText;

    private UISubItem_UserCurrency_Presenter _presenter;

    public void Init()
    {
        if (_presenter == null)
        {
            _presenter = new UISubItem_UserCurrency_Presenter(this);
        }

        _presenter.Init();
    }

    public void SetText(string userGoldText, string currentFatigue, string maxFatigue, string dia)
    {
        UserGoldText.text = userGoldText;
        UserFatigueText.text = $"{currentFatigue} / {maxFatigue}";
        UserDiaText.text = dia;
    }
}

public class UISubItem_UserCurrency_Presenter : Presenter
{
    private UISubItem_UserCurrency _view;

    public UISubItem_UserCurrency_Presenter(UISubItem_UserCurrency view)
    {
        _view = view;

        GameManager.Instance.FatigueSystem.OnFatigueChange -= SetText;
        GameManager.Instance.FatigueSystem.OnFatigueChange += SetText;
    }

    public void Init()
    {
        SetText();
    }

    /// <summary>
    /// 유저가 갖고있는 골드, 다이아, 피로도를 보여준다
    /// </summary>
    public void SetText()
    {
        List<ServerUserInventoryData> list = DataManager.Instance.ServerDataSystem.UserInventoryList;
        ServerUserInventoryData gold = list.Find(x => x.ItemId == Define.ItemGold);
        string userGoldText = gold != null ? gold.Quantity.ToString() : "0";

        // 피로도
        string currentFatigue = DataManager.Instance.ServerDataSystem.UserFatigueData.CurrentFatigue.ToString();
        string maxFatigue = DataManager.Instance.ServerDataSystem.UserFatigueData.MaxFatigue.ToString();

        ServerUserInventoryData dia = list.Find(x => x.ItemId == Define.ItemDia);
        string userDiaText = dia != null ? dia.Quantity.ToString() : "0";

        _view.SetText(userGoldText, currentFatigue, maxFatigue, userDiaText);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UISubItem_EggInventoryItem 알 누르면 UIPopup_EggInventory 에 인벤토리 인덱스 전달
/// </summary>
public class UIPopup_EggInventory : UIPopup
{
    [Header("Button")]
    public Button GotoIncubatorButton;
    public Button SmallNutrientButton;
    public Button MediumNutrientButton;
    public Button LargeNutrientButton;
    public Button StartHatchButton;

    [Header("GameObject")]
    public GameObject Right;
    public GameObject ScrollView;

    [Header("Text")]
    public TextMeshProUGUI SmallNutrientCountText;
    public TextMeshProUGUI MediumNutrientCountText;
    public TextMeshProUGUI LargeNutrientCountText;
    public TextMeshProUGUI NutrientSumText;
    public TextMeshProUGUI NeedGaugeText;

    [Header("Image")]
    public Image EggItemImage;
    public Image GaugeImage;

    public UIPopup_EggInventory_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();

        Presenter = new UIPopup_EggInventory_Presenter(this);

        Presenter.Init();
    }

    public void SetSprite(Sprite egg)
    {
        EggItemImage.sprite = egg;
    }

    public void SetButton(Action OnGotoIncubator, Action OnSmallNutrient,
        Action OnMediumNutrient, Action OnLargeNutrient, Action OnStartHatch)
    {
        GotoIncubatorButton.onClick.AddListener(() => { OnGotoIncubator(); OnUIClickSound(); });
        SmallNutrientButton.onClick.AddListener(() => { OnSmallNutrient(); OnUIClickSound(); });
        MediumNutrientButton.onClick.AddListener(() => { OnMediumNutrient(); OnUIClickSound(); });
        LargeNutrientButton.onClick.AddListener(() => { OnLargeNutrient(); OnUIClickSound(); });
        StartHatchButton.onClick.AddListener(() => { OnStartHatch(); OnUIClickSound(); });
    }

    public void SetText(string smallNutrientCount, string mediumNutrientCount, string largeNutrientCount, string nutrientSum, string needGauge)
    {
        SmallNutrientCountText.text = smallNutrientCount;
        MediumNutrientCountText.text = mediumNutrientCount;
        LargeNutrientCountText.text = largeNutrientCount;
        NutrientSumText.text = nutrientSum;
        NeedGaugeText.text = " / " + needGauge;
    }

    public void SetBar(float fillAmount)
    {
        GaugeImage.fillAmount = fillAmount;
    }
}

public class UIPopup_EggInventory_Presenter
{
    private UIPopup_EggInventory _view;

    private int _nutrientSum = 0;
    private int _smallNutrientCount = 0;
    private int _mediumNutrientCount = 0;
    private int _largeNutrientCount = 0;

    private int _needGauge = 0;

    private ServerUserEggData _selectedEgg;

    public UIPopup_EggInventory_Presenter(UIPopup_EggInventory view)
    {
        _view = view;

        SetButton();
    }

    public void Init()
    {
        DestoryAllISubtems();

        ServerUserEggData egg = GetFirstEgg();

        if (egg != null)
        {
            SelectEgg(egg);
        }

        SetSubItem();
    }

    public void SelectEgg(ServerUserEggData egg)
    {
        _selectedEgg = egg;

        GameManager.Instance.HatchingSystem.SelectedEgg(egg);

        SetAddedNutrientToZero();
        Refresh();
    }

    private void Refresh()
    {
        SetBar();
        SetText();
        SetSprite();
    }

    private void SetSubItem()
    {
        _view.ScrollView.SetActive(true);

        List<ServerUserEggData> userInventoryList = DataManager.Instance.ServerDataSystem.UserEggDataList;

        foreach (ServerUserEggData data in userInventoryList)
        {
            UISubItem_EggInventoryItem subItem = UIManager.Instance.InstanciateSubItem<UISubItem_EggInventoryItem>(_view.Right.transform);

            subItem.Presenter.SetSubItem(data, this._view);
        }
    }

    private void SetText()
    {
        string smallNutrientCount = _smallNutrientCount.ToString();
        string mediumNutrientCount = _mediumNutrientCount.ToString();
        string largeNutrientCount = _largeNutrientCount.ToString();

        if (_selectedEgg != null)
        {
            _needGauge = DataManager.Instance.ReadOnlyDataSystem.EggInfo[_selectedEgg.EggId].NeedGauge;
        }
        else
        {
            _needGauge = 0;
        }
        string needGauge = _needGauge.ToString();

        _nutrientSum = Mathf.Clamp(_nutrientSum, 0, _needGauge);
        string nutrientSum = _nutrientSum.ToString();

        _view.SetText(smallNutrientCount, mediumNutrientCount, largeNutrientCount, nutrientSum, needGauge);
    }

    private void SetSprite()
    {
        Sprite egg;
        if (_selectedEgg != null)
        {
            egg = ResourceManager.Instance.LoadSprite("Icon/EggIcon/eggs", _selectedEgg.EggId);
        }
        else
        {
            egg = ResourceManager.Instance.Load<Sprite>($"Icon/EggIcon/EMPTYEGG");
        }

        _view.SetSprite(egg);
    }

    private void SetBar()
    {
        float gauge = 0;

        if (_needGauge != 0)
        {
            gauge = (float)_nutrientSum / _needGauge;
        }

        _view.SetBar(gauge);
    }

    private ServerUserEggData GetFirstEgg()
    {
        if (DataManager.Instance.ServerDataSystem.UserEggDataList.Count == 0)
        {
            return null;
        }

        return DataManager.Instance.ServerDataSystem.UserEggDataList[0];
    }

    private void DestoryAllISubtems()
    {
        foreach (Transform child in _view.Right.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void SetAddedNutrientToZero()
    {
        _smallNutrientCount = 0;
        _mediumNutrientCount = 0;
        _largeNutrientCount = 0;
        _nutrientSum = 0;
    }

    private void SetButton()
    {
        _view.SetButton(OnExit, OnSmallNutrient, OnMediumNutrient, OnLargeNutrient, OnClickStartHatchButton);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
    }

    private bool IsUsingNutrient()
    {
        if (_selectedEgg == null)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("알이 없습니다!");
            return false;
        }
        if (_nutrientSum >= _needGauge)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("영양제가 충분합니다!");

            return false;
        }

        return true;
    }

    /// <summary>
    /// 초급 영양제 사용 버튼 
    /// </summary>
    private void OnSmallNutrient()
    {
        if (IsUsingNutrient() == false)
        {
            return;
        }

        int smallvalue = DataManager.Instance.ReadOnlyDataSystem.ItemInfo[Define.ItemSmallNutrient].Value;

        ServerUserInventoryData data = DataManager.Instance.ServerDataSystem.UserInventoryList.Find(x => x.ItemId == Define.ItemSmallNutrient);
        if (data == null)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("초급 영양제가 없습니다!");
            return;
        }

        int smallNutrient = data.Quantity;


        if (smallNutrient <= _smallNutrientCount)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("초급 영양제 부족!");

            return;
        }

        _smallNutrientCount++;
        _nutrientSum += smallvalue;

        Refresh();
    }

    /// <summary>
    /// 중급 영양제 사용 버튼
    /// </summary>
    private void OnMediumNutrient()
    {
        if (IsUsingNutrient() == false)
        {
            return;
        }

        int mediumValue = DataManager.Instance.ReadOnlyDataSystem.ItemInfo[Define.ItemMediumNutrient].Value;

        ServerUserInventoryData data = DataManager.Instance.ServerDataSystem.UserInventoryList.Find(x => x.ItemId == Define.ItemMediumNutrient);
        if (data == null)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("중급 영양제가 없습니다!");
            return;
        }

        int mediumNutrient = data.Quantity;

        if (mediumNutrient <= _mediumNutrientCount)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("중급 영양제 부족!");

            return;
        }

        _mediumNutrientCount++;
        _nutrientSum += mediumValue;
        

        Refresh();
    }

    /// <summary>
    /// 고급 영양제 사용 버튼
    /// </summary>
    private void OnLargeNutrient()
    {
        if (IsUsingNutrient() == false)
        {
            return;
        }

        int largeValue = DataManager.Instance.ReadOnlyDataSystem.ItemInfo[Define.ItemLargeNutrient].Value;

        ServerUserInventoryData data = DataManager.Instance.ServerDataSystem.UserInventoryList.Find(x => x.ItemId == Define.ItemLargeNutrient);
        if (data == null)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("고급 영양제가 없습니다!");
            return;
        }

        int largeNutrient = data.Quantity;

        if (largeNutrient <= _largeNutrientCount)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("고급 영양제 부족!");

            return;
        }

        _largeNutrientCount++;
        _nutrientSum += largeValue;

        Refresh();
    }

    /// <summary>
    /// 1 영양제 부족 -> 영양제 부족 팝업
    /// 2 영양제 꽉 참 -> 부화 시작 누르면 부화
    /// </summary>
    private void OnClickStartHatchButton()
    {
        if (_nutrientSum < _needGauge)
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("영양제를 더 넣어주세요!");

            return;
        }

        // 게이지 차서 부화 시작 
        else if (_nutrientSum >= _needGauge)
        {
            StartHatchingProcess();
        }
    }

    private async void StartHatchingProcess()
    {
        await LoadingManager.Instance.RunWithLoading(async () =>
        {

            await DataManager.Instance.ServerDataSystem.StartHatchEgg(new StartHatchingBody
            {
                Id = GameManager.Instance.HatchingSystem.IncubatorId,
                UserId = DataManager.Instance.ServerDataSystem.User.UserId,
                UserEggId = _selectedEgg.Id,
                EggId = _selectedEgg.EggId,
                Materials = new ItemMaterial[]
                {
                new ItemMaterial { ItemId = Define.ItemSmallNutrient, Quantity = _smallNutrientCount },
                new ItemMaterial { ItemId = Define.ItemMediumNutrient, Quantity = _mediumNutrientCount },
                new ItemMaterial { ItemId = Define.ItemLargeNutrient, Quantity = _largeNutrientCount }
                },
                hoursToHatch = DataManager.Instance.ReadOnlyDataSystem.EggInfo[_selectedEgg.EggId].HatchTime

            });
        },
        onLoaded: () =>
        {
            GameManager.Instance.HatchingSystem.StartHatching(_smallNutrientCount, _mediumNutrientCount, _largeNutrientCount);

            UIManager.Instance.DestoryUIPopup(_view);
            UIManager.Instance.FindUIPopup<UIPopup_Incubator>().gameObject.SetActive(false);
            UIManager.Instance.FindUIPopup<UIPopup_Incubator>().gameObject.SetActive(true);

            if (_smallNutrientCount > 0)
            {
                EventManager.Instance.Invoke(EventType.OnUseSmallNutrient, this, _smallNutrientCount);
            }
            if (_mediumNutrientCount > 0)
            {
                EventManager.Instance.Invoke(EventType.OnUseMediumNutrient, this, _mediumNutrientCount);
            }
            if (_largeNutrientCount > 0)
            {
                EventManager.Instance.Invoke(EventType.OnUseLargeNutrient, this, _largeNutrientCount);
            }
        },
        onError: (message) =>
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"UIPopup_EggInventory::OnClickStartHatchButton()  실패: {message}", "UpdateFatigueOnServer", sceneName);
        });
    }
}
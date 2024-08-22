using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UISubItem_SelectEnvironmentTypeButton : UISubItem
{
    private Dictionary<EnvironmentType, string> environmentTypeNames = new Dictionary<EnvironmentType, string>
    {
        { EnvironmentType.Grassland, "초원" },
        { EnvironmentType.Desert, "사막" },
        { EnvironmentType.Beach, "해변" },
        { EnvironmentType.SnowField, "설원" }
    };

    public EnvironmentType EnvironmentType { get; set; }

    private UIPopup_DungeonTypeSelect _dungeonSelectType;
    private UIPopup_DungeonSelectStage _dungeonSelectStage;

    private Button _button;
    public Image Image;
    public TextMeshProUGUI DungeonType;
    private int _currentType;

    protected override void Awake()
    {
        base.Awake();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
        _button.onClick.AddListener(() => OnUIClickSound());

        Transform parent = transform.parent;
        while (parent != null)
        {
            _dungeonSelectType = parent.GetComponent<UIPopup_DungeonTypeSelect>();
            if (_dungeonSelectType != null)
            {
                break;
            }
            parent = parent.parent;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public void Initialize(Define.EnvironmentType environmentType)
    {
        EnvironmentType = environmentType;

        if (environmentTypeNames.TryGetValue(environmentType, out string environmentName))
        {
            DungeonType.text = environmentName;
        }
        else
        {
            DungeonType.text = environmentType.ToString();
        }

        Sprite sprite = ResourceManager.Instance.Load<Sprite>($"Icon/EnvironmentTypeIcon/{(int)environmentType}");
        Image.sprite = sprite;
        //   _dungeonSelectStageScreen = stageScreen; 매개변수 GameObject stageScreen, 
        //  _setActiveFalseObj = setActiveFalseObj; 매개변수 GameObject setActiveFalseObj
        _currentType = (int)environmentType;
    }

    private void OnButtonClicked()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonSelectStage>();
        _dungeonSelectStage = FindObjectOfType<UIPopup_DungeonSelectStage>();
        _dungeonSelectStage.CurrentEnvironmentType = _currentType;
    }
}
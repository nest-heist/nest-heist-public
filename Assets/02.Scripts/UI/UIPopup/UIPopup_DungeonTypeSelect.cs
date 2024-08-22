using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_DungeonTypeSelect : UIPopup
{
    [Header("Button")]
    public Button ExitBtn;

    [Header("Script")]
    public UISubItem_SelectEnvironmentTypeButton SelectEnvironmentTypeButton;

    public Transform ScreenTransform;

    public UIScene_DungeonTypeSelect_Presnenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIScene_DungeonTypeSelect_Presnenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Presenter.Init();
    }

    public UISubItem_SelectEnvironmentTypeButton CreateButton()
    {
        return Instantiate(SelectEnvironmentTypeButton, ScreenTransform);
    }

    public void SetButton(Action onExit)
    {
        ExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
    }
}

public class UIScene_DungeonTypeSelect_Presnenter
{
    private UIPopup_DungeonTypeSelect _view;

    public UIScene_DungeonTypeSelect_Presnenter(UIPopup_DungeonTypeSelect view)
    {
        _view = view;
    }

    public void Init()
    {
        CreateEnvironmentTypeButtons();
        SetButton();
    }

    void CreateEnvironmentTypeButtons()
    {
        // Define.EnvironmentType의 enum 값을 배열로 변환
        var environmentTypes = (Define.EnvironmentType[])System.Enum.GetValues(typeof(Define.EnvironmentType));

        foreach (var environmentType in environmentTypes)
        {
            CreateButton(environmentType);
        }
    }

    public void CreateButton(Define.EnvironmentType environmentType)
    {
        UISubItem_SelectEnvironmentTypeButton newButton = _view.CreateButton();
        newButton.Initialize(environmentType);
    }
    private void SetButton()
    {
        _view.SetButton(OnExit);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }

}
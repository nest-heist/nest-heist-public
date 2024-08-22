using UnityEngine;
using UnityEngine.UI;

public class UIPopup_DungeonTutorial : UIPopup
{
    [Header("Button")]
    public Button CloseBtn;

    private UIPopup_Tutorial_Presenter _presenter;


    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0f;
    }

    protected override void OnEnable()
    {
        SetButton();
    }

    public void SetButton()
    {
        CloseBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            UIManager.Instance.DestoryAllUIPopup();
        });
    }
}
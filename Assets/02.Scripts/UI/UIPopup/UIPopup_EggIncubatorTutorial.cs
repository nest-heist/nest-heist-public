using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_EggIncubatorTutorial : UIPopup
{
    [Header("Button")]
    public Button NextBtn;
    public Button CloseBtn;
    public Button PrevBtn;

    [Header("Screen")]
    public GameObject Scrren1;
    public GameObject Scrren2;

    public int CurrentPage;
    private UIPopup_EggIncubatorTutorial_Presenter _presenter;


    protected override void Awake()
    {
        base.Awake();
        CurrentPage = 0;
        _presenter = new UIPopup_EggIncubatorTutorial_Presenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _presenter.Init();
    }

    public void SetButton(Action nextBtn, Action closeBtn, Action prevBtn)
    {
        NextBtn.onClick.AddListener(() => nextBtn());
        CloseBtn.onClick.AddListener(() => closeBtn());
        PrevBtn.onClick.AddListener(() => prevBtn());
    }
}

public class UIPopup_EggIncubatorTutorial_Presenter
{
    private UIPopup_EggIncubatorTutorial _view;

    public UIPopup_EggIncubatorTutorial_Presenter(UIPopup_EggIncubatorTutorial view)
    {
        _view = view;
    }

    public void Init()
    {
        SetButton();
    }

    private void SetButton()
    {
        _view.SetButton(OnNextPage, OnExit, OnPrevPage);
    }

    private void OnNextPage()
    {
        if (_view.CurrentPage == 0)
        {
            _view.CurrentPage++;
            _view.Scrren1.SetActive(false);
            _view.Scrren2.SetActive(true);
        }
        else if (_view.CurrentPage == 1)
        {
            _view.CurrentPage = 0;
            _view.Scrren1.SetActive(true);
            _view.Scrren2.SetActive(false);
        }
    }

    private void OnPrevPage()
    {
        if (_view.CurrentPage == 0)
        {
            _view.CurrentPage = 1;
            _view.Scrren1.SetActive(true);
            _view.Scrren2.SetActive(false);
        }
        else if (_view.CurrentPage == 1)
        {
            _view.CurrentPage--;
            _view.Scrren1.SetActive(false);
            _view.Scrren2.SetActive(true);
        }
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/// <summary>
/// </summary>
public class UIScene_StartScene : UIScene
{
    [SerializeField] private Button _startButton;

    private UIScene_StartScene_Presenter _presenter;

    protected override void Awake()
    {
        // base.Awake();
        UIManager.Instance.SetUIScene(gameObject, true);
    }

    /// <summary>
    /// </summary>
    protected override void Start()
    {
        // base.Start();
        UIManager.Instance.SetUIScene(gameObject, true);

        _presenter = new UIScene_StartScene_Presenter(this);
    }

    public void SetButton(Action OnStartButtonClicked)
    {
        _startButton.onClick.AddListener(() => OnStartButtonClicked());
    }
}

public class UIScene_StartScene_Presenter : Presenter
{
    private UIScene_StartScene _view;

    public UIScene_StartScene_Presenter(UIScene_StartScene view)
    {
        _view = view;

        _view.SetButton(OnStartButtonClicked);
    }

    private async void OnStartButtonClicked()
    {
        await LoadingManager.Instance.RunWithLoading(DataManager.Instance.ServerDataSystem.InitData,
        onLoaded: () =>
        {
            GameManager.Instance.Load();
            SceneManager.LoadScene(Define.LobbyScene);
        },
        onError: (error) =>
        {
            Logging.LogError(error);
        });
    }
}

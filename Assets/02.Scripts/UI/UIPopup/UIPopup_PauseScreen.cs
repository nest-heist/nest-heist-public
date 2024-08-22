using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPopup_PauseScreen : UIPopup
{
    [Header("Button")]
    public Button RetryBtn;
    public Button GoToTheLobbyBtn;
    public Button AreaOutBtn;
    public Button ExitBtn;
    public Button FatigueTestBtn;
    public Button GuidBtn;

    public UIPopup_PauseScreen_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIPopup_PauseScreen_Presenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Presenter.Init();
        Time.timeScale = 0f;
    }

    public void SetButton(Action onRetry, Action onExit, Action onReplay, Action onFatigueExit, Action onGuid)
    {
        RetryBtn.onClick.AddListener(() => { onRetry(); OnUIClickSound(); });
        GoToTheLobbyBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
        AreaOutBtn.onClick.AddListener(() => { onReplay(); OnUIClickSound(); }  );
        ExitBtn.onClick.AddListener(() =>   { onReplay(); OnUIClickSound(); });
        FatigueTestBtn.onClick.AddListener(() => { onFatigueExit(); OnUIClickSound(); });
        GuidBtn.onClick.AddListener(() => { onGuid(); OnUIClickSound(); });
    }
}

public class UIPopup_PauseScreen_Presenter
{
    private UIPopup_PauseScreen _view;

    public UIPopup_PauseScreen_Presenter(UIPopup_PauseScreen view)
    {
        _view = view;
    }

    public void Init()
    {
        SetButton();
    }

    private void SetButton()
    {
        _view.SetButton(OnRetry, OnExit, OnReplay, OnFatigueExit, OnGuid);
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// 로비에서 바로 나오면 피로도 안 깎기, 피로도 업데이트는 필요하다
    /// </summary>
    private void OnExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(Define.LobbyScene);
        AudioManager.Instance.PlayBGM("IntroLobbyBGM");
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(0);
    }

    private void OnReplay()
    {
        Time.timeScale = 1f;
        UIManager.Instance.DestoryUIPopup(_view);
    }

    /// <summary>
    /// 게임 클리어 하면 사용한 피로도 서버에 보내기
    /// 임시 버튼으로 피로도 소모 테스트 용이다.
    /// </summary>
    private void OnFatigueExit()
    {
        Time.timeScale = 1f;
        UIManager.Instance.DestoryUIPopup(_view);

        int fatigue = DungeonBattleManager.Instance.InfoData.Fatigue;
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(fatigue);
    }

    // TODO : 수정
    private void OnGuid()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonTutorial>();
    }
}

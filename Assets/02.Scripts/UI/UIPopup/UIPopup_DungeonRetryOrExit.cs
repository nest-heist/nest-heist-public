using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPopup_DungeonRetryOrExit : UIPopup
{
    [Header("Button")]
    public Button RetryBtn;
    public Button ExitBtn;

    [Header("TextObject")]
    public GameObject PlayerDieText;
    public GameObject PartnerMonstersDieText;

    UIPopup_DungeonRetryOrExit_Presenter Presenter;

    protected override void Awake()
    {
        PlayerDieText.SetActive(false);
        PartnerMonstersDieText.SetActive(false);
        base.Awake();
        Presenter = new UIPopup_DungeonRetryOrExit_Presenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Presenter.Init();
        StartCoroutine(TimeZero());
        SetOnText();
    }

    private IEnumerator TimeZero()
    {
        yield return new WaitForSeconds(1f); 
        Time.timeScale = 0f;
    }

    public void SetButton(Action onRetry,Action onExit)
    {
        RetryBtn.onClick.AddListener(() => { onRetry(); OnUIClickSound(); });
        ExitBtn.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
    }

    public void SetOnText()
    {
        if(GameManager.Instance.UserSystem.PlayerDie)
        {
            PlayerDieText.SetActive(true);
        }

        if(GameManager.Instance.UserMonsterSystem.AllPartnerMonsterDie)
        {
            PartnerMonstersDieText.SetActive(true);
        }    
    }
}

public class UIPopup_DungeonRetryOrExit_Presenter
{
    private UIPopup_DungeonRetryOrExit _view;
    public UIPopup_DungeonRetryOrExit_Presenter(UIPopup_DungeonRetryOrExit view)
    {
        _view = view;
    }

    public void Init()
    {
        SetButton();
    }
    
    private void SetButton()
    { 
        _view.SetButton(OnRetry, OnExit);
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(Define.DungeonScene);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnExit()
    {
        Time.timeScale = 1f;

        // TODO : UIManager.Instance.DestoryAllUIPopup();
        AudioManager.Instance.PlayBGM("IntroLobbyBGM");
        SceneManager.LoadScene(Define.LobbyScene);
    }
}
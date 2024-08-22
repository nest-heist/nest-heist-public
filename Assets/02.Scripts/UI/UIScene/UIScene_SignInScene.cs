using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// StartScene 전체 다 넣기
/// </summary>
public class UIScene_SignInScene : UIScene
{
    [SerializeField] private Button _googleLoginButton;
    private UIScene_SignInScene_Presenter _presenter;

    protected override void Awake()
    {
        // base.Awake();
        UIManager.Instance.SetUIScene(gameObject, true);
    }

    /// <summary>
    /// Loader에서 Awake()로 데이터 블러와서 Start()에서 시작하기
    /// </summary>
    protected override void Start()
    {
        // base.Start();
        UIManager.Instance.SetUIScene(gameObject, true);

        _presenter = new UIScene_SignInScene_Presenter(this);
        _presenter.Init();

        // 버튼을 처음에는 비활성화
        _googleLoginButton.interactable = false;

        // 2초 후 버튼을 활성화하는 코루틴 시작
        StartCoroutine(ActivateButtonAfterDelay(2f));

        _googleLoginButton.onClick.AddListener(OnGoogleLoginButtonClicked);
    }

    private System.Collections.IEnumerator ActivateButtonAfterDelay(float delay)
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 버튼 활성화
        _googleLoginButton.interactable = true;
    }

    private void OnGoogleLoginButtonClicked()
    {
        // 버튼을 비활성화하여 중복 클릭을 막음
        _googleLoginButton.interactable = false;

        FirebaseManager.Instance.AuthSystem.GoogleSignInClick(() =>
        {
            SceneManager.LoadScene(Define.TitleScene);
        });
    }
}

public class UIScene_SignInScene_Presenter : Presenter
{
    private UIScene_SignInScene _view;

    public UIScene_SignInScene_Presenter(UIScene_SignInScene view)
    {
        _view = view;
    }

    public void Init()
    {

    }
}

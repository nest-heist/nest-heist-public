using System;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestFireBaseAuth : MonoBehaviour
{
    public Button GotoStartSceneButton;
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField TokenInput;
    public TMP_Text error;

    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    void Start()
    {
        GotoStartSceneButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(Define.TitleScene);
        });
    }

    public void Create()
    {
        FirebaseManager.Instance.AuthSystem.CreateUserWithEmail(email.text, password.text, (str) =>
        {

            Enqueue(() =>
            {
                error.text = str;
            });
        });
    }

    public void Login()
    {
        FirebaseManager.Instance.AuthSystem.SignInWithEmail(email.text, password.text, (token) =>
        {
            Enqueue(() =>
            {
                TokenInput.text = token;
                GotoStartSceneButton.gameObject.SetActive(true);
            });
        });
    }

    public void FastLogin()
    {
        FirebaseManager.Instance.AuthSystem.SignInWithEmail("test@test.test", "test1234", (token) =>
        {
            Enqueue(() =>
            {
                TokenInput.text = token;
                GotoStartSceneButton.gameObject.SetActive(true);
            });
        });
    }

    public void FastLogin2()
    {
        FirebaseManager.Instance.AuthSystem.SignInWithEmail("123@123.123", "123123123", (token) =>
        {
            Enqueue(() =>
            {
                TokenInput.text = token;
                GotoStartSceneButton.gameObject.SetActive(true);
            });
        });
    }

    public void LogOut()
    {
        FirebaseManager.Instance.AuthSystem.SignOut();
    }

    private void Update()
    {
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    public static void Enqueue(Action action)
    {
        if (action == null)
        {
            return;
        }

        _executionQueue.Enqueue(action);
    }

    private void OnDestroy()
    {
        _executionQueue.Clear();
    }
}

using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthSystem
{
    public string GoogleWebAPI = "617696192829-rhg5it6j24lv4fegid6o9j7ak3l9occr.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private FirebaseUser user;
    private GoogleSignInConfiguration configuration;


    public string IdToken { get; private set; }
    public string UserName { get; private set; }
    public string UserEmail { get; private set; }
    public string UserPhotoUrl { get; private set; }

    public void Init(Action onSessionActive = null, Action onSessionInactive = null)
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true,
            RequestEmail = true
        };
        auth = FirebaseAuth.DefaultInstance;

        // 현재 유저가 로그인되어 있는지 확인
        user = auth.CurrentUser;

        if (user != null)
        {
            // 유저가 존재하면, 세션 유지
            Debug.Log("User already signed in. Bypassing login.");
            onSessionActive?.Invoke();
        }
        else
        {
            // 유저가 없으면 로그인 버튼을 표시
            Debug.Log("No user signed in. Showing login button.");
            onSessionInactive?.Invoke();
        }
    }

    #region Google Login


    public void IsVisibleGoogleAuth()
    {
        UserName = user.DisplayName;
        UserEmail = user.Email;
        UserPhotoUrl = user.PhotoUrl?.ToString();

        user.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
        {
            if (tokenTask.IsFaulted || tokenTask.IsCanceled)
            {
                return;
            }

            IdToken = tokenTask.Result;
        });
    }
    public void GoogleSignInClick(Action onSuccessCallback = null)
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => OnGoogleAuthenticatedFinished(task, onSuccessCallback));
    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task, Action onSuccessCallback)
    {
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Google authentication failed.");
            return;
        }

        Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
        {
            if (authTask.IsFaulted || authTask.IsCanceled)
            {
                Debug.LogError("Firebase authentication failed.");
                return;
            }

            user = auth.CurrentUser;
            if (user != null)
            {
                UserName = user.DisplayName;
                UserEmail = user.Email;
                UserPhotoUrl = user.PhotoUrl?.ToString();

                user.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
                {
                    if (tokenTask.IsFaulted || tokenTask.IsCanceled)
                    {
                        Debug.LogError("Failed to retrieve IdToken.");
                        return;
                    }

                    IdToken = tokenTask.Result;
                    Debug.Log("ID Token: " + IdToken);

                    onSuccessCallback?.Invoke(); // 성공 시 콜백 호출
                });
            }
        });
    }
    #endregion

    #region Email/Password Authentication

    public void CreateUserWithEmail(string email = "test@test.test", string password = "test1234", Action<string> callback = null)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                callback?.Invoke("Failed to create user.");
                return;
            }

            user = task.Result.User;
            callback?.Invoke("User created successfully.");
        });
    }

    public void IsVisibleEmailAuth()
    {
        UserEmail = user.Email;
        user.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
        {
            if (tokenTask.IsFaulted || tokenTask.IsCanceled)
            {
                return;
            }

            IdToken = tokenTask.Result;
            Debug.Log("ID Token: " + IdToken);
        });
    }
    public void SignInWithEmail(string email, string password, Action<string> callback)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                callback?.Invoke("Failed to sign in.");
                return;
            }

            user = task.Result.User;
            if (user != null)
            {
                UserEmail = user.Email;

                user.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
                {
                    if (tokenTask.IsFaulted || tokenTask.IsCanceled)
                    {
                        callback?.Invoke("Failed to retrieve IdToken.");
                        return;
                    }

                    IdToken = tokenTask.Result;
                    Debug.Log("ID Token: " + IdToken);
                    callback?.Invoke(IdToken);
                });
            }
        });
    }

    #endregion
    public void SignOut()
    {
        user = null;
        IdToken = null;
        UserName = null;
        UserEmail = null;
        UserPhotoUrl = null;
#if UNITY_EDITOR
#else
        GoogleSignIn.DefaultInstance.SignOut();
#endif
        auth = FirebaseAuth.DefaultInstance;
# if UNITY_EDITOR
        SceneManager.LoadScene(Define.TestLoginScene);
#else
        SceneManager.LoadScene(Define.TitleScene);
        GoogleSignIn.DefaultInstance.SignOut();
#endif
        auth.SignOut();
    }
}
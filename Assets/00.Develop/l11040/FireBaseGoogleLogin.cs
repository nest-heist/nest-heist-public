using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireBaseGoogleLogin : MonoBehaviour
{
    public string GoogleWebAPI = "617696192829-rhg5it6j24lv4fegid6o9j7ak3l9occr.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public TMP_InputField TokenInput;
    public TextMeshProUGUI UserNameTxt, UserEmailTxt;

    // UserInfoSystem에서 이미지 로드해서 사용한다. 이부분 사용 X 
    public Image UserProfilePic;

    public string imageURL;
    public GameObject LoginScreen, ProfileScreen;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true,
        };
    }
    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.Log("OnGoogleAuthenticatedFinished task.IsFaulted");
        }
        else if (task.IsCanceled)
        {
            Debug.Log("OnGoogleAuthenticatedFinished task.IsCanceled");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithCredentialAsync task.IsCanceled");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("SignInWithCredentialAsync task.IsFaulted");
                    return;
                }
                user = auth.CurrentUser;
                UserNameTxt.text = user.DisplayName;
                UserEmailTxt.text = user.Email;

                user.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
                {
                    if (tokenTask.IsCanceled)
                    {
                        Debug.Log("TokenAsync was canceled.");
                        return;
                    }
                    if (tokenTask.IsFaulted)
                    {
                        Debug.Log("TokenAsync encountered an error: " + tokenTask.Exception);
                        return;
                    }

                    string idToken = tokenTask.Result;
                    Debug.Log("ID Token: " + idToken);

                    TokenInput.text = idToken;
                });

                LoginScreen.SetActive(false);
                ProfileScreen.SetActive(true);
                /*StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));*/
            });
        }
    }

    string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return imageURL;
    }

    /*IEnumerator LoadImage(string imageUri)
    {
        WWW www = new WWW(imageUri);
        yield return www;
        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }*/
}


using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 파이어베이스에서 구글 로그인 시 받아오는 유저 정보
/// 서버데이터 매니저에서 갖고있지 않고 FirebaseManager - AuthSystem 에서 갖고있는 데이터 모아놨다.
/// 유저 사진, 이름
/// </summary>
public class UserInfoSystem
{
    public Sprite UserProfileSprite;
    public string UserProfileName;

    public event Action OnUserProfileSpriteLoaded;

    public async void Init()
    {
        // 테스트 할 때 파이어베이스 로그인 X 
        if(string.IsNullOrEmpty(FirebaseManager.Instance.AuthSystem.UserName))
        {
            UserProfileName = "파이어베이스 모바일확인";
            UserProfileSprite = ResourceManager.Instance.Load<Sprite>($"Icon/PlayerIcon/PID00001");
            return;
        }

        UserProfileName = FirebaseManager.Instance.AuthSystem.UserName;
        await LoadImage(FirebaseManager.Instance.AuthSystem.UserPhotoUrl);
    }

    private async Task LoadImage(string imageUri)
    {
        // 이미지 로드
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUri))
        {
            UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            // 이미지가 없을 경우 기본 이미지로 대체
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"UserInfoSystem::LoadImage() webRequest.error: {webRequest.error}");
                UserProfileSprite = ResourceManager.Instance.Load<Sprite>($"Icon/PlayerIcon/PID00001");
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                UserProfileSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                OnUserProfileSpriteLoaded?.Invoke();
            }
        }
    }
}
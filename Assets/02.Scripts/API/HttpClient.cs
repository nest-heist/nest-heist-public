using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpClient
{
    private readonly string baseUrl = "http://20.39.199.14";
    // private readonly string baseUrl = "http://localhost:3000";

    public async Task GetRequest(string uri, string authToken, Action<string> onSuccess = null, Action<string> onFailure = null)
    {
        string fullUrl = $"{baseUrl}{uri}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fullUrl))
        {
            string clientVersion = Application.version;
            webRequest.SetRequestHeader("x-client-version", clientVersion);
            webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");
            var operation = webRequest.SendWebRequest();


            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                if (webRequest.responseCode == 409)
                {
                    VersionErrorAlert();
                }
                else
                {
                    Debug.LogError($"Error: [{uri}] {webRequest.error}");
                    onFailure?.Invoke(webRequest.error);
                }
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    public async Task PostRequest(string uri, string jsonData, string authToken, Action<string> onSuccess = null, Action<string> onFailure = null)
    {
        string fullUrl = $"{baseUrl}{uri}";
        using (UnityWebRequest webRequest = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // 현재 프로젝트의 버전 헤더 추가
            string clientVersion = Application.version;
            webRequest.SetRequestHeader("x-client-version", clientVersion);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {authToken}");


            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                if (webRequest.responseCode == 409)
                {
                    VersionErrorAlert();
                }
                else
                {
                    Debug.LogError($"Error: [{uri}] {webRequest.error}\n Data: [{jsonData}]");
                    onFailure?.Invoke(webRequest.error);
                }
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
    private void VersionErrorAlert()
    {
        LoadingManager.Instance.HideLoadingScreen();
        UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
        popup.CloseButton.gameObject.SetActive(false);
        popup.SetText("최신 버전으로 업데이트 해주세요");
    }
}

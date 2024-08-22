using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

public class ApiService
{
    private readonly HttpClient httpClient;

    public ApiService()
    {
        httpClient = new HttpClient();
    }

    public async Task<T> GetData<T>(string uri, Action<T> onSuccess = null, Action<string> onFailure = null)
    {
        TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

        await httpClient.GetRequest(uri, FirebaseManager.Instance.AuthSystem.IdToken, json =>
        {
            if (!string.IsNullOrEmpty(json))
            {
                T data = JsonConvert.DeserializeObject<T>(json);
                onSuccess?.Invoke(data);
                tcs.SetResult(data);
            }
            else
            {
                string error = "No data received.";
                onFailure?.Invoke(error);
                tcs.SetException(new Exception(error));
            }
        }, error =>
        {
            onFailure?.Invoke(error);
            tcs.SetException(new Exception(error));
        });

        return await tcs.Task;
    }

    public async Task<TResponse> PostData<TPost, TResponse>(string uri, TPost postData, Action<TResponse> onSuccess = null, Action<string> onFailure = null)
    {
        string jsonData = JsonConvert.SerializeObject(postData);
        TaskCompletionSource<TResponse> tcs = new TaskCompletionSource<TResponse>();

        await httpClient.PostRequest(uri, jsonData, FirebaseManager.Instance.AuthSystem.IdToken, json =>
        {
            if (!string.IsNullOrEmpty(json))
            {
                TResponse data = JsonConvert.DeserializeObject<TResponse>(json);
                onSuccess?.Invoke(data);
                tcs.SetResult(data);
            }
            else
            {
                string error = "No data received.";
                onFailure?.Invoke(error);
                tcs.SetException(new Exception(error));
            }
        }, error =>
        {
            /*string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, error, "PostData", sceneName);*/

            onFailure?.Invoke(error);
            tcs.SetException(new Exception(error));
        });

        return await tcs.Task;
    }
}

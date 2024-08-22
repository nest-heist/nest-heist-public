using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class DBSystem
{
    private DatabaseReference databaseReference;
    public void Init()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void SendErrorLog(string uid, string errorMessage, string stackTrace, string sceneName,  object extraData = null)
    {
        ErrorLog log = new ErrorLog(errorMessage, stackTrace, sceneName, extraData);
        string json = JsonUtility.ToJson(log);
        string logKey = databaseReference.Child("error_logs").Child(uid).Child("log_entries").Push().Key;
        databaseReference.Child("error_logs").Child(uid).Child("log_entries").Child(logKey).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"데이터 쓰기 오류: {task.Exception}");
            }
            else
            {
                Debug.Log("데이터 쓰기 성공!");
            }
        });
    }
}
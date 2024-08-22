using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RequestQueueSystem
{
    private Queue<Func<Task>> _requestQueue = new Queue<Func<Task>>();
    private bool _isProcessing = false;

    // 요청을 큐에 추가
    public void EnqueueRequest(Func<Task> request)
    {
        _requestQueue.Enqueue(request);
        if (!_isProcessing)
        {
            ProcessQueue();
        }
    }

    // 큐에 쌓인 요청을 순차적으로 처리
    private async void ProcessQueue()
    {
        _isProcessing = true;

        while (_requestQueue.Count > 0)
        {
            var request = _requestQueue.Dequeue();
            try
            {
                await request();
            }
            catch (Exception ex)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string uid = DataManager.Instance.ServerDataSystem.User.UserId;
                FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
                Logging.LogError(ex.Message);
            }
        }

        _isProcessing = false;
    }
}
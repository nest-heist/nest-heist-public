using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadingManager : Singleton<LoadingManager>
{
    private GameObject _loadingPanelInstance;
    public GameObject loadingPanelPrefab;

    private List<Task> _tasks = new List<Task>();
    private bool _isLoading = false;
    private EventSystem _eventSystem;

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
        loadingPanelPrefab = ResourceManager.Instance.Load<GameObject>("Prefabs/Loading/Loading");
        CreateLoadingPanel();
    }

    private void CreateLoadingPanel()
    {
        if (_loadingPanelInstance == null)
        {
            try
            {
                _loadingPanelInstance = Instantiate(loadingPanelPrefab);
            }
            catch (Exception ex)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string uid = DataManager.Instance.ServerDataSystem.User.UserId;
                FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, _loadingPanelInstance);
                Logging.LogError(ex.Message);
            }
            DontDestroyOnLoad(_loadingPanelInstance);
            _loadingPanelInstance.SetActive(false);
        }
    }

    public IEnumerator RunWithLoadingCoroutine(IEnumerator coroutine, bool isVisiblePanel = false, Action onLoaded = null, Action<string> onError = null)
    {
        // Show the loading screen
        ShowLoadingScreen(isVisiblePanel);

        // Start the coroutine and wait for it to finish
        yield return StartCoroutine(coroutine);

        // Hide the loading screen
        HideLoadingScreen(isVisiblePanel);

        // Call the onLoaded callback if provided
        onLoaded?.Invoke();
    }

    public async Task RunWithLoading(Func<Task> apiRequest, bool isVisiblePanel = false, Action onLoaded = null, Action<string> onError = null)
    {
        // 로딩 시작
        ShowLoadingScreen(isVisiblePanel);

        // API 요청을 리스트에 추가하고 실행
        Task task = apiRequest();
        _tasks.Add(task);

        try
        {
            await task;

            // 요청이 끝난 후 리스트에서 제거
            _tasks.Remove(task);

            if (task.IsCompletedSuccessfully)
            {
                // 요청이 성공적으로 완료된 경우
                onLoaded?.Invoke();
            }
            else if (task.IsFaulted)
            {
                // 요청이 실패한 경우
                onError?.Invoke(task.Exception?.Flatten().InnerExceptions[0].Message);
            }
            else if (task.IsCanceled)
            {
                // 요청이 취소된 경우
                onError?.Invoke("Request was canceled.");
            }
        }
        catch (Exception ex)
        {
            // 요청 실패 시 오류 처리
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, task);
            Logging.LogError(ex.Message);
            _tasks.Remove(task);
            onError?.Invoke(ex.Message);
        }

        // 만약 모든 요청이 끝났다면, 로딩 종료
        if (_tasks.Count == 0)
        {
            HideLoadingScreen(isVisiblePanel);
        }
    }

    private void ShowLoadingScreen(bool isVisiblePanel = false)
    {
        if (!_isLoading)
        {
            if (isVisiblePanel)
            {
                CreateLoadingPanel();
                _loadingPanelInstance.SetActive(true);
            }
            _isLoading = true;
            // EventSystem 비활성화
            _eventSystem = EventSystem.current;
            if (_eventSystem != null)
            {
                _eventSystem.enabled = false;
            }
        }
    }

    public void HideLoadingScreen(bool isVisiblePanel = false)
    {
        if (_isLoading)
        {
            if (isVisiblePanel)
            {
                _loadingPanelInstance.SetActive(false);
            }
            _isLoading = false;
            // EventSystem 활성화
            if (_eventSystem != null)
            {
                _eventSystem.enabled = true;
            }
        }
    }
}

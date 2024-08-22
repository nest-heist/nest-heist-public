using Firebase;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

class FirebaseManager : Singleton<FirebaseManager>
{

    public AuthSystem AuthSystem { get; private set; }
    public DBSystem DBSystem { get; private set; }
    public CrashlyticsSystem CrashlyticsSystem { get; private set; }

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }
    void Init()
    {
        AuthSystem = new AuthSystem();
        DBSystem = new DBSystem();
        CrashlyticsSystem = new CrashlyticsSystem();
        InitFirebase();
    }

    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                AuthSystem.Init(
                onSessionActive: () =>
                {
                    // 세션이 활성화된 상태 -> 바로 게임 시작
#if UNITY_EDITOR
                    AuthSystem.IsVisibleEmailAuth();
#else
                    AuthSystem.IsVisibleGoogleAuth();
#endif
                    SceneManager.LoadScene(Define.TitleScene);
                },
                onSessionInactive: () =>
                {
                });
                DBSystem.Init();
                CrashlyticsSystem.Init();
            }
            else
            {
                Logging.LogError($"Firebase initialization error: {dependencyStatus}");
            }
        });
    }
}
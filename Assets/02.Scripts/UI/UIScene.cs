using System;

/// <summary>
/// 씬에 바로 배치되는 UI는 이 클래스를 상속받아서 사용
/// </summary>
public class UIScene : UICommon
{
    /// <summary>
    /// 기본적으로 UIScene은 Awake에서 초기화한다
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        try
        {
            UIManager.Instance.SetUIScene(gameObject);
        }
        catch (Exception e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    /// <summary>
    /// UIScene_Dungeon 은 DungeonBattleManager때문에 Start에서 초기화한다 
    /// </summary>
    protected override void Start()
    {
        base.Start();

        try
        {
            UIManager.Instance.SetUIScene(gameObject);
        }
        catch (NullReferenceException e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
}

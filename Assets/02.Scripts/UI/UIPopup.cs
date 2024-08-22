using System;
using UnityEngine;

/// <summary>
/// 팝업UI는 이 클래스를 상속받아서 사용
/// </summary>
public class UIPopup : UICommon
{
    /// <summary>
    /// soringOrder에 맞게 정렬 해준다 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        try
        {
            UIManager.Instance.SetUIPopup(gameObject);
        }
        catch (Exception e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDestory()
    {
        base.OnDestory();
    }
}

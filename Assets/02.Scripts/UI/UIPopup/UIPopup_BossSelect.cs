using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPopup_BossSelect : UIPopup
{
    [Header("Button")]
    public Button Boss1;
    public Button Boss2;
    public Button ExitBtn;

    protected override void Start()
    {
        base.Start();
        SetButton();
    }

    void SetButton()
    {
        Boss1.onClick.AddListener(() => { OnBoss1(); });

        Boss2.onClick.AddListener(() => { OnBoss2(); });

        ExitBtn.onClick.AddListener(() => { UIOffAndOnScene(); OnUIClickSound(); });
    }

    void OnBoss1()
    {
        // 계속 클라에서 켜두는 경우 서버에 피로도 업데이트 필요
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(0);

        // 피로도 부족 
        if (GameManager.Instance.FatigueSystem.FatigueData.CurrentFatigue < 20)
        {
            UIPopup_Warning warning = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            warning.SetText("피로도가 부족합니다.");
        }
        // 피로도 충분 : 나갈 때 피로도 깎기
        else
        {
            SceneManager.LoadScene(Define.Boss1Stage); OnUIClickSound();
            GameManager.Instance.DungeonStageSystem.BossSelectedId = "Boss001HeadIcon";
            GameManager.Instance.DungeonStageSystem.BossEggId = "EGG00001";
        }
    }

    void OnBoss2()
    {
        // 계속 클라에서 켜두는 경우 서버에 피로도 업데이트 필요
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(0);

        // 피로도 부족 
        if (GameManager.Instance.FatigueSystem.FatigueData.CurrentFatigue < 20)
        {
            UIPopup_Warning warning = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            warning.SetText("피로도가 부족합니다.");
        }
        // 피로도 충분 : 나갈 때 피로도 깎기
        else
        {
            SceneManager.LoadScene(Define.Boss2Stage); OnUIClickSound();
            GameManager.Instance.DungeonStageSystem.BossSelectedId = "Boss002HeadIcon";
            GameManager.Instance.DungeonStageSystem.BossEggId = "EGG00002";
        }

    }

    void UIOffAndOnScene()
    {
        UIManager.Instance.DestoryUIPopup(this);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
    }
}
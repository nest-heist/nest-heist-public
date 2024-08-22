using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_GrowUpScreen : UIPopup
{
    public UIPopup_GrowUpScreen_Presenter Presenter;
    public UIPopup_MonsterInventory Inventroy;
    public UISubItem_GrowUpMonsterButton MonsterButton;

    [Header("Button")]
    public Button OnExit;
    public Button ConfirmRemoveMonsterBtn;

    [Header("Transform")]
    public Transform Right;

    [Header("Text")]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ExpCountText;

    [Header("Image")]
    public Image LvUpMonsterImage;
    public Image LvExpBar;

    public int ExpCount;
    public int DisPlayCurrentExpCount;

    protected override void Awake()
    {
        base.Awake();
        DisPlayCurrentExpCount = 0;
        Inventroy = FindObjectOfType<UIPopup_MonsterInventory>();
        Presenter = new UIPopup_GrowUpScreen_Presenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Presenter.Init();
    }

    public void UpdateLevelText()
    {
        LevelText.text = Presenter.CurrentMonster.UserData.Level.ToString();
    }

    public void UpdateExpCountText()
    {
        ExpCountText.text = DisPlayCurrentExpCount.ToString();
    }

    public void SetButton(Action onExit, Action growUpConfirm)
    {
        OnExit.onClick.AddListener(() => { onExit(); OnUIClickSound(); });
        ConfirmRemoveMonsterBtn.onClick.AddListener(() => { growUpConfirm(); OnUIClickSound(); });
    }

    public void SetImage(Sprite sprite)
    {
        LvUpMonsterImage.sprite = sprite;
    }

    public UISubItem_GrowUpMonsterButton CreateMonsterButton()
    {
        var button = Instantiate(MonsterButton, Right);
        button.Presenter = new UISubItem_GrowUpMonsterButton_Presenter(button);
        return button;
    }
}

public class UIPopup_GrowUpScreen_Presenter
{
    private UIPopup_GrowUpScreen _view;
    public UserMonster CurrentMonster;
    private int _currentTotalExp = 0;

    public UIPopup_GrowUpScreen_Presenter(UIPopup_GrowUpScreen view)
    {
        _view = view;
    }

    public void Init()
    {
        SetButton();
        GenerateButtonsForMonster();
        SetImage();
    }

    public void GenerateButtonsForMonster()
    {
        ClearButtons();

        CurrentMonster = _view.Inventroy.Presenter.CurrentMonster;
        int currentMonsterGroupId = GetMonsterGroupId(CurrentMonster.UserData.MonsterId);

        foreach (var userMonsterPair in GameManager.Instance.UserMonsterSystem.UserMonsterList)
        {
            UserMonster userMonster = userMonsterPair.Value;
            int userMonsterGroupId = GetMonsterGroupId(userMonster.UserData.MonsterId);

            // 그룹 ID가 -1이거나, 현재 몬스터와 그룹이 다르면 제외
            if (userMonsterGroupId == -1 || currentMonsterGroupId != userMonsterGroupId)
            {
                continue;
            }

            // 동일 그룹에 속하는 몬스터만 추가
            if (userMonster.UserData.Id != CurrentMonster.UserData.Id)
            {
                AddMonsterButton(userMonster);
            }
        }

        _currentTotalExp = CurrentMonster.UserData.EXP;
        _view.ExpCount = _currentTotalExp;
        _view.UpdateLevelText();
        _view.DisPlayCurrentExpCount = 0;
        _view.UpdateExpCountText();
        UpdateExpBar();
    }


    // MonsterId를 기반으로 그룹 ID를 반환하는 메서드
    private int GetMonsterGroupId(string monsterId)
    {
        int monsterNumber = ExtractMonsterNumber(monsterId);

        // 1번 몬스터는 그룹 ID 1로 설정
        if (monsterNumber == 1)
        {
            return 1;
        }

        // 101~150 범위는 슬라임 그룹으로 설정 (예: 그룹 ID 2)
        if (monsterNumber >= 101 && monsterNumber <= 150)
        {
            return 2;
        }

        // 그 외의 경우 그룹 ID를 -1로 설정하여 그룹에서 제외
        return -1;
    }

    private int ExtractMonsterNumber(string monsterId)
    {
        // "MON00101"에서 "101" 부분을 추출
        if (int.TryParse(monsterId.Substring(5), out int number))
        {
            return number;
        }
        return -1;
    }

    public void ClearButtons()
    {
        foreach (Transform child in _view.Right.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddMonsterButton(UserMonster userMonster)
    {
        _view.CreateMonsterButton().Presenter.SetSubItem(userMonster);
    }

    private void SetImage()
    {
        Sprite monsterSprite = ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", CurrentMonster.AllStatData.BaseStat.MonsterId);
        _view.SetImage(monsterSprite);
    }

    private void SetButton()
    {
        _view.SetButton(OnExit, OnConfirmRemoveMonster);
    }

    private void OnExit()
    {
        UIManager.Instance.DestoryUIPopup(_view);
        UIManager.Instance.UIScene.gameObject.SetActive(true);
        _view.Inventroy.Presenter.Initialize();
    }

    public int GetCurrentTotalExp()
    {
        return _currentTotalExp;
    }

    public void AddExp(int exp)
    {
        _currentTotalExp += exp;
    }

    public void AddDisplayExp(int exp)
    {
        _view.DisPlayCurrentExpCount += exp;
    }

    private async void OnConfirmRemoveMonster()
    {
        List<string> selectedExpMonsterIds = new List<string>();

        foreach (Transform child in _view.Right)
        {
            var button = child.GetComponent<UISubItem_GrowUpMonsterButton>();
            if (button != null && button.IsChecked())
            {
                selectedExpMonsterIds.Add(button.GetSelectedExpMonsterId());
            }
        }

        if (selectedExpMonsterIds.Count > 0)
        {
            int maxExp = DataManager.Instance.ReadOnlyDataSystem.MonsterLevelUp[CurrentMonster.UserData.Level].SumOfExp;

            await LoadingManager.Instance.RunWithLoading(async () =>
            {
                await DataManager.Instance.ServerDataSystem.UserMonsterLevelUp(new UserMonsterLevelUpBody
                {
                    userMonsterId = CurrentMonster.UserData.Id,
                    maxExp = maxExp,
                    expMonsterIds = selectedExpMonsterIds.ToArray()
                });
            }, onLoaded: () =>
            {
                GenerateButtonsForMonster();
            },
            onError: (message) =>
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string uid = DataManager.Instance.ServerDataSystem.User.UserId;
                FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"레벨업 실패: {message}", "OnConfirmRemoveMonster", sceneName);

                GenerateButtonsForMonster();
            });
        }
    }

    public void UpdateExpBar()
    {
        _view.LvExpBar.fillAmount = (float)_view.ExpCount / (float)DataManager.Instance.ReadOnlyDataSystem.MonsterLevelUp[CurrentMonster.UserData.Level].SumOfExp;
    }
}

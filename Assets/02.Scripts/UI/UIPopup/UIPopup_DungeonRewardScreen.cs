using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using static Define;


public class UIPopup_DungeonRewardScreen : UIPopup
{
    public UIScene_DungeonRewardScreen_Presenter Presenter;

    [Header("Image")]
    public Image PlayerImage;
    public Image CurrentExpBar;

    [Header("Transform")]
    public Transform RewardListContainer;
    public Transform RewardEggListContainer;

    [Header("GameObject")]
    public GameObject RewardItemPrefab;

    [Header("Button")]
    public Button GoToLobby;

    [Header("Text")]
    public TextMeshProUGUI NextLevelUpNeedCount;

    [Header("AnimationSpeed")]
    public float AnimeSpeed;

    public Player Player;
    public int UpExpCount;

    protected override void Awake()
    {
        base.Awake();
        Presenter = new UIScene_DungeonRewardScreen_Presenter(this);
    }

    public void Init(DungeonDropSystem dropManager)
    {
        Player = FindObjectOfType<Player>();
        Presenter.Init(dropManager);             
    }

    public void DisplayRewards(Dictionary<string, int> itemQuantities, List<ServerUserEggData> eggs)
    {
        Presenter.UpdataStageClear();
        foreach (Transform child in RewardListContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in RewardEggListContainer)
        {
            Destroy(child.gameObject);
        }

        // 집계된 아이템을 UI에 표시
        foreach (var item in itemQuantities)
        {
            GameObject rewardItem = Instantiate(RewardItemPrefab, RewardListContainer);
            rewardItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.Value}";

            Image itemImage = rewardItem.GetComponent<Image>();
            if (itemImage != null)
            {
                Sprite itemSprite = ResourceManager.Instance.Load<Sprite>($"Icon/ItemIcon/{item.Key}");
                if (itemSprite != null)
                {
                    itemImage.sprite = itemSprite;
                }
                else
                {
                    Logging.LogError($"이미지를 찾을 수 없음: {item.Key}");
                }
            }
        }

        // 집계된 알을 UI에 표시
        foreach (var egg in eggs)
        {
            GameObject rewardItem = Instantiate(RewardItemPrefab, RewardEggListContainer);
            rewardItem.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.Instance.ReadOnlyDataSystem.EggInfo[egg.EggId].Name;

            Image itemImage = rewardItem.GetComponent<Image>();
            if (itemImage != null)
            {
                Sprite itemSprite = ResourceManager.Instance.LoadSprite($"Icon/EggIcon/eggs", egg.EggId);
                
                if (itemSprite != null)
                {
                    itemImage.sprite = itemSprite;
                }
                else
                {
                    Logging.LogError($"이미지를 찾을 수 없음: {egg.EggId}");
                }
            }
        }
        StartCoroutine(Presenter.HandleExpAnimation());
    }

    public void SetButton(Action goToLobby)
    {
        GoToLobby.onClick.AddListener(() => { goToLobby(); OnUIClickSound(); });
    }

    public void SetImage(Sprite sprite)
    {
        PlayerImage.sprite = sprite;
    }
}

public class UIScene_DungeonRewardScreen_Presenter
{
    public int PreviousLevel;
    private float _animationStartExp;
    private float _animationEndExp;
    private UIPopup_DungeonRewardScreen _view;
    private DungeonDropSystem _dropManager;
    private List<ServerUserDungeonProgressData> _currentDataList;

    public UIScene_DungeonRewardScreen_Presenter(UIPopup_DungeonRewardScreen view)
    {
        _view = view;
    }

    public void Init(DungeonDropSystem dropManager)
    {
        _dropManager = dropManager;
        _currentDataList = DeepCopyList(DataManager.Instance.ServerDataSystem.UserDungeonProgressList);
        SetButton();
        ShowRewards();
        SetImage();
    }

    private void ShowRewards()
    {
        if (_dropManager != null)
        {
            List<ServerUserInventoryData> acquiredItems = _dropManager.AddItemList;
            List<ServerUserEggData> acquiredEggs = _dropManager.TempEggDataList;

            Dictionary<string, int> itemQuantities = new Dictionary<string, int>();

            foreach (var item in acquiredItems)
            {
                if (item.ItemId == "ITE00001")
                {
                    EventManager.Instance.Invoke(EventType.OnGetGold, this, item.Quantity);

                    continue;
                }

                if (itemQuantities.ContainsKey(item.ItemId))
                {
                    itemQuantities[item.ItemId] += item.Quantity;
                }
                else
                {
                    itemQuantities[item.ItemId] = item.Quantity;
                }
            }

            _view.DisplayRewards(itemQuantities, acquiredEggs);
        }
    }

    private void SetButton()
    {
        _view.SetButton(GoToLobby);
    }

    /// <summary>
    /// 던전 클리어하면 InfoData에 있는 피로도를 서버에 업데이트
    /// </summary>
    private void GoToLobby()
    {
        Time.timeScale = 1f;
        GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(DungeonBattleManager.Instance.InfoData.Fatigue);
        SceneManager.LoadScene(Define.LobbyScene);
    }

    private void SetImage()
    {
        _view.SetImage(GameManager.Instance.UserInfoSystem.UserProfileSprite);
    }

    public IEnumerator HandleExpAnimation()
    {
        PreviousLevel = _view.Player.UserData.Level;
        float remainingExp = (float)_view.UpExpCount;

        while (remainingExp > 0)
        {
            int currentLevel = _view.Player.UserData.Level;
            float currentExp = _view.Player.UserData.Exp;
            float requiredExp = DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[currentLevel].SumOfExp;

            if (currentExp + remainingExp >= requiredExp)
            {
                // 레벨업 가능한 경우
                float excessExp = currentExp + remainingExp - requiredExp;
                _animationStartExp = currentExp / requiredExp;
                _animationEndExp = 1f;

                // 애니메이션 재생
                yield return AnimateExpBar(_animationStartExp, _animationEndExp, _view.AnimeSpeed);

                // 레벨업 처리
                GameManager.Instance.UserSystem.UpExp((int)(requiredExp - currentExp), _view.Player);
                remainingExp = excessExp;

                // 레벨업 후 초기화
                PreviousLevel = _view.Player.UserData.Level;
                _view.Player.UserData.Exp = 0;
                _view.NextLevelUpNeedCount.text = $"레벨업!";
            }
            else
            {
                // 레벨업 불가능한 경우
                _animationStartExp = currentExp / requiredExp;
                _animationEndExp = (currentExp + remainingExp) / requiredExp;

                // 애니메이션 재생
                yield return AnimateExpBar(_animationStartExp, _animationEndExp, _view.AnimeSpeed);

                // 경험치 추가
                GameManager.Instance.UserSystem.UpExp((int)remainingExp, _view.Player);
                remainingExp = 0;
            }

            _view.NextLevelUpNeedCount.text = $"다음 레벨업까지 {DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_view.Player.UserData.Level].SumOfExp - _view.Player.UserData.Exp} 남았습니다\n" +
                                              $"현재 레벨{_view.Player.UserData.Level}";
            _view.CurrentExpBar.fillAmount = (float)_view.Player.UserData.Exp / DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_view.Player.UserData.Level].SumOfExp;
        }
    }

    private IEnumerator AnimateExpBar(float startFillAmount, float endFillAmount, float duration)
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + duration;

        while (Time.unscaledTime < endTime)
        {
            float t = (Time.unscaledTime - startTime) / duration;
            float currentFillAmount = Mathf.Lerp(startFillAmount, endFillAmount, t);
            _view.CurrentExpBar.fillAmount = currentFillAmount;
            yield return null;
        }

        _view.CurrentExpBar.fillAmount = endFillAmount;
    }

    private UpdateStageBody CreateUpdateStageClearBody(int newStageId)
    {
        return new UpdateStageBody
        {
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            EnvironmentType = DungeonBattleManager.Instance.InfoData.EnvironmentType,
            NextStageId = newStageId
        };
    }

    public async void UpdataStageClear()
    {
        var userId = DataManager.Instance.ServerDataSystem.User.UserId;
        var newStageId = DungeonBattleManager.Instance.InfoData.Stage + 1;
        var shouldUpdate = false;

        foreach (var userProgress in DataManager.Instance.ServerDataSystem.UserDungeonProgressList)
        {
            if (userProgress.UserId == userId && userProgress.EnvironmentType == DungeonBattleManager.Instance.InfoData.EnvironmentType)
            {
                if (newStageId > userProgress.StageId)
                {
                    shouldUpdate = true;
                    break;
                }
            }
        }

        if (shouldUpdate)
        {
            await LoadingManager.Instance.RunWithLoading(async () =>
            {
                await DataManager.Instance.ServerDataSystem.UpdateDungeonStage(CreateUpdateStageClearBody(newStageId));
            },
            onLoaded: () =>
            {
                foreach (var userProgress in DataManager.Instance.ServerDataSystem.UserDungeonProgressList)
                {
                    if (userProgress.UserId == userId && userProgress.EnvironmentType == DungeonBattleManager.Instance.InfoData.EnvironmentType)
                    {
                        if (newStageId > userProgress.StageId)
                        {
                            userProgress.StageId = newStageId;
                        }
                        break;
                    }
                }
            },
            onError: (message) => { });
        }
    }

    private List<ServerUserDungeonProgressData> DeepCopyList(List<ServerUserDungeonProgressData> originalList)
    {
        List<ServerUserDungeonProgressData> newList = new List<ServerUserDungeonProgressData>();
        foreach (var item in originalList)
        {
            ServerUserDungeonProgressData newItem = new ServerUserDungeonProgressData
            {
                UserId = item.UserId,
                EnvironmentType = item.EnvironmentType,
                StageId = item.StageId,
            };
            newList.Add(newItem);
        }
        return newList;
    }
}
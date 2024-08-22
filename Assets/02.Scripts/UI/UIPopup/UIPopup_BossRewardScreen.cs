using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPopup_BossRewardScreen : UIPopup
{
    public UIPopup_BossRewardScreen_Presenter Presenter;

    [Header("Image")]
    public Image PlayerImage;
    public Image CurrentExpBar;

    [Header("Transform")]
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

        // TODO : 임시로 경험치 삽입, 추후 보스 클리어 경험치 정해지면 변경해야함
        UpExpCount = 100;

        Presenter = new UIPopup_BossRewardScreen_Presenter(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Presenter.Init();
        Player = BossBattleManager.Instance.PlayerCs;
        Time.timeScale = 0f;
        DisplayRewards();
        OnGoToLobby();
    }

    public void DisplayRewards()
    {
        foreach (Transform child in RewardEggListContainer)
        {
            Destroy(child.gameObject);
        }

        // 집계된 알을 UI에 표시
        GameObject rewardItem = Instantiate(RewardItemPrefab, RewardEggListContainer);
        rewardItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{GameManager.Instance.DungeonStageSystem.BossEggId}의 알";

        Image itemImage = rewardItem.GetComponent<Image>();
        if (itemImage != null)
        {
            Sprite itemSprite = ResourceManager.Instance.LoadSprite($"Icon/EggIcon/eggs", GameManager.Instance.DungeonStageSystem.BossEggId);
            if (itemSprite != null)
            {
                itemImage.sprite = itemSprite;
            }
            else
            {
                Logging.LogError($"이미지를 찾을 수 없음: {GameManager.Instance.DungeonStageSystem.BossEggId}");
            }
        }

        StartCoroutine(Presenter.HandleExpAnimation());
    }

    public void OnGoToLobby()
    {
        GoToLobby.onClick.AddListener(() => { GameManager.Instance.FatigueSystem.UpdateFatigueOnServer(20);  Time.timeScale = 1.0f;  SceneManager.LoadScene(Define.LobbyScene); });
    }

    public void SetImage(Sprite sprite)
    {
        PlayerImage.sprite = sprite;
    }
}

public class UIPopup_BossRewardScreen_Presenter
{
    private UIPopup_BossRewardScreen _view;
    public int PreviousLevel;
    private float _animationStartExp;
    private float _animationEndExp;

    public UIPopup_BossRewardScreen_Presenter(UIPopup_BossRewardScreen view)
    {
        _view = view;
    }
    public void Init()
    {
        SetImage();
        GetBossEgg();
    }

    private void SetImage()
    {
        _view.SetImage(GameManager.Instance.UserInfoSystem.UserProfileSprite);
    }

    private void GetBossEgg()
    {
        ServerUserEggData newEggData = new ServerUserEggData
        {
            // 새로운 아이디를 임의로 구현
            Id = Guid.NewGuid().ToString(),
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            EggId = GameManager.Instance.DungeonStageSystem.BossEggId,
            AcquiredAt = "",
            HatchStart = "",
            IsHatched = false
        };

        BossBattleManager.Instance.DropSystem.TempEggDataList.Add(newEggData);
        BossBattleManager.Instance.DropSystem.ItemAndEggDataOnServer();
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
}
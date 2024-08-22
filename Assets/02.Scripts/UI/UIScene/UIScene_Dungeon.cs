using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

/// <summary>
/// 던전 씬 UI
/// </summary>
public class UIScene_Dungeon : UIScene, IScene
{
    [Header("Image")]
    public Image PlayerImage;
    public Image PlayerHPImage;
    public Image[] PartyHPImage;
    public Image[] PartyCooldownImage;
    public Image[] PartyBtnImages;
    public Image[] PartySkillIImages;
    public Image SkillBtnImage;

    [Header("Text")]
    public TextMeshProUGUI SkillLeftTimeText;
    public TextMeshProUGUI ActionModeText;
    public TextMeshProUGUI PlayerNameText;

    [Header("Button")]
    public Button SkillButton;
    public Button TempButton;
    public Button ActionModeButton;
    public Button[] PartyMonsterBtns;

    [Header("GameObject")]
    public GameObject CombatImage;
    public GameObject NoCombatImage;
    public GameObject WarningText;

    public UIScene_Dungeon_Presenter Presenter;
    public IBattleManager BattleManager;

    public void SetBattleManager(IBattleManager battleManager)
    {
        UIManager.Instance.SetUIScene(gameObject);
        WarningText.SetActive(false);
        BattleManager = battleManager;
        Presenter = new UIScene_Dungeon_Presenter(this);
        Presenter.Init();

        SkillEvents.OnSkillCooldownUpdate -= UpdateSkillCooldownText;
        SkillEvents.OnSkillCooldownUpdate += UpdateSkillCooldownText;

        Tutorial dungeonTutorial = GameManager.Instance.PageTutorialSystem.GetTutorialById("dungeon");
        if (dungeonTutorial != null)
        {
            GameManager.Instance.PageTutorialSystem.ShowDungeonTutorialIfNotShown(dungeonTutorial);
        }
        StartCoroutine(SetOnWarningText());
    }

    public void SetPlayerHPBar(float playerHP)
    {
        PlayerHPImage.fillAmount = playerHP;
    }

    public void SetPartyHPBar(int index, float monsterHP)
    {
        PartyHPImage[index].fillAmount = monsterHP;
    }

    public void SetPartyCooldownImage(int index, float cooldownTime)
    {
        PartyCooldownImage[index].fillAmount = 1 - cooldownTime;
    }

    public void SetSprite(Sprite playerSprite, int monsterCount)
    {
        PlayerImage.sprite = GameManager.Instance.UserInfoSystem.UserProfileSprite;

        for (int i = 0; i < monsterCount && i < PartyMonsterBtns.Length; i++)
        {
            PartyBtnImages[i].sprite = Presenter.MonsterSprite(i);
        }
    }

    public void SetText(string userName)
    {
        PlayerNameText.text = userName;
    }

    public void SetSkillCooldownText(string skillCooldownText)
    {
        SkillLeftTimeText.text = skillCooldownText;
    }

    private void UpdateSkillCooldownText(float remainingCooldown)
    {
        string cooldownText = remainingCooldown > 0.01f ? remainingCooldown.ToString("F1") : " ";
        SetSkillCooldownText(cooldownText);
    }
    public void SetButton(Action OnPause, Action OnSkill, Action OnActionMode, Action[] OnChangePartyMonster)
    {
        TempButton.onClick.AddListener(() => OnPause());
        SkillButton.onClick.AddListener(() => OnSkill());
        ActionModeButton.onClick.AddListener(() => OnActionMode());

        for (int i = 0; i < OnChangePartyMonster.Length && i < PartyMonsterBtns.Length; i++)
        {
            int index = i;
            PartyMonsterBtns[i].onClick.AddListener(() => OnChangePartyMonster[index]());
        }
    }

    public void SetGameObject(bool isCombat)
    {
        CombatImage.SetActive(isCombat);
        NoCombatImage.SetActive(!isCombat);
    }

    public void RemovePartyMonsterButtonClickListener(int index)
    {
        if (index < 0 || index >= PartyMonsterBtns.Length) return;

        Button targetButton = PartyMonsterBtns[index];

        // 기존의 모든 리스너 제거
        targetButton.onClick.RemoveAllListeners();
        // 비활성화된 버튼으로 시각적으로 나타냄
        PartyBtnImages[index].color = Color.gray;
        PartyHPImage[index].fillAmount = 0f;
        PartyCooldownImage[index].gameObject.SetActive(false);
    }

    public void HighlightCurrentMonsterButton(int currentIndex)
    {
        // 모든 버튼의 색상과 투명도를 반투명하고 회색으로 설정
        foreach (var buttonImage in PartyBtnImages)
        {
            Image image = buttonImage;
            Color color = Color.gray;
            color.a = 0.5f;
            image.color = color;
        }

        // 현재 몬스터 버튼의 색상과 투명도를 원래 상태로 설정
        if (currentIndex >= 0 && currentIndex < PartyMonsterBtns.Length)
        {
            Image image = PartyBtnImages[currentIndex];
            Color color = Color.white;
            color.a = 1f;
            image.color = color;
        }
    }

    public void SkillButtonOnOff()
    {
        if (Presenter.MonsterAIHandler.GetNearEnemy())
        {
            Image image = SkillBtnImage;
            Color color = Color.white;
            color.a = 1f;
            image.color = color;
        }

        if (!Presenter.MonsterAIHandler.GetNearEnemy() || Presenter.Monster.SkillHandler.GetIsCooldown())
        {
            Image image = SkillBtnImage;
            Color color = Color.gray;
            color.a = 0.5f;
            image.color = color;
        }
    }

    public void SetSkillButtonSprite(Sprite sprite)
    {
        SkillBtnImage.sprite = sprite;
    }

    public void SetPartySkillImage(int index, Sprite sprite)
    {
        PartySkillIImages[index].sprite = sprite;
    }

    public void OnChangePartyMonster(int currentindex)
    {
        Presenter.OnChangePartyMonster(currentindex);
    }

    private IEnumerator SetOnWarningText()
    {
        yield return new WaitUntil(() => DungeonBattleManager.Instance.PhaseCount == 2);

        RectTransform rectTransform = WarningText.GetComponent<RectTransform>();

        Vector2 startPosition = new Vector2(rectTransform.rect.width, -242);
        Vector2 endPosition = new Vector2(0, -242); // Y값을 동일하게 유지

        // 애니메이션 시간
        float animationDuration = 0.5f;

        rectTransform.anchoredPosition = startPosition;
        WarningText.SetActive(true);

        // 슬라이드 인 애니메이션 (startPosition -> endPosition)
        rectTransform.DOAnchorPos(endPosition, animationDuration).SetEase(Ease.OutCubic);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationDuration);

        // 5초 대기
        yield return new WaitForSeconds(5f);

        // 슬라이드 아웃 애니메이션 (endPosition -> startPosition)
        rectTransform.DOAnchorPos(startPosition, animationDuration).SetEase(Ease.InCubic);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationDuration);

        // 경고 텍스트를 비활성화
        WarningText.SetActive(false);
    }
}

public class UIScene_Dungeon_Presenter : Presenter
{
    private UIScene_Dungeon _view;

    int _partyMonsterCount;
    private int _currentIndex;
    public BaseMonster Monster;
    public PartnerMonsterAIHandler MonsterAIHandler;
    Player _player;
    private List<SkillHandler> _monsterSkillHandlers = new List<SkillHandler>();
    private List<BaseMonster> _baseMonsters = new List<BaseMonster>();
    private List<PartnerMonsterAIHandler> _partnerAIHandlers = new List<PartnerMonsterAIHandler>();
    private float _updateInterval = 0.1f;

    public UIScene_Dungeon_Presenter(UIScene_Dungeon view)
    {
        _view = view;
        _partyMonsterCount = DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds.Length;

        for (int i = 0; i < _partyMonsterCount; i++)
        {
            var obj = _view.BattleManager.AllPartnerMonster[i];

            var monster = obj.GetComponent<BaseMonster>();
            _baseMonsters.Add(monster);

            var skillHandler = monster.GetComponent<SkillHandler>();
            _monsterSkillHandlers.Add(skillHandler);

            var aiHandler = obj.GetComponent<PartnerMonsterAIHandler>();
            _partnerAIHandlers.Add(aiHandler);
        }

        Monster = _baseMonsters[0];
        MonsterAIHandler = _partnerAIHandlers[0];

        _player = _view.BattleManager.Player.GetComponent<Player>();
        SetButton();
    }

    public void Init()
    {
        _currentIndex = 0;
        SetPartyMonsterUI(_partyMonsterCount);
        SetBar();
        SetText();
        SetSkillCooldownText();
        SetSprite();
        _player.Health.OnHPChangedEvent += UpdatePlayerHP;
        Monster.HealthSystem.OnHPChangedEvent += UpdataePartyHPBar;
        _view.HighlightCurrentMonsterButton(_currentIndex);
        _view.StartCoroutine(UpdateCooldownImages());

        for (int i = 0; i < _partyMonsterCount; i++)
        {
            _view.SetPartySkillImage(i, SetSpriteSkillBtn(i));
        }

    }

    private void SetSprite()
    {
        // TODO : 플레이어 이미지 고정 값 
        string playerImageId = "PID00001";
        Sprite playerSprite = ResourceManager.Instance.Load<Sprite>($"Icon/PlayerIcon/{playerImageId}");

        _view.SetSprite(playerSprite, _partyMonsterCount);
        _view.SetSkillButtonSprite(SetSpriteSkillBtn(0));
    }

    public Sprite MonsterSprite(int index)
    {
        string[] monsterIds = DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds;

        // 배열 인덱스 검사
        if (index < 0 || index >= monsterIds.Length)
        {
            Logging.LogError($"배열 범위 이탈: {index}. 범위: 0 to {monsterIds.Length - 1}");
            return null;
        }

        string monsterId = monsterIds[index];

        // UserMonsterList에서 몬스터 객체 가져오기
        if (!GameManager.Instance.UserMonsterSystem.UserMonsterList.TryGetValue(monsterId, out UserMonster partnerMonster))
        {
            Logging.LogError($"유저몬스터 리스트에 몬스터 ID가 없음: {monsterId}");
            return null;
        }

        string monsterimageId = partnerMonster.UserData.MonsterId;

        Sprite partnerMonsterSprite = ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", monsterimageId);
        return partnerMonsterSprite;
    }

    public Sprite SetSpriteSkillBtn(int index)
    {
        string monsterSkillId = _baseMonsters[index].SkillHandler.Info.Id;

        Sprite monsterSkillSprite = ResourceManager.Instance.LoadSprite("Icon/SkillIcon/", monsterSkillId);
        return monsterSkillSprite;
    }



    private void SetText()
    {
        string userName = GameManager.Instance.UserInfoSystem.UserProfileName;

        _view.SetText(userName);
    }

    private void SetSkillCooldownText()
    {
        string cooldownText = Monster.SkillHandler.Info.CoolDown.ToString();
        if (Monster.SkillHandler.GetIsCooldown())
        {
            _view.SetSkillCooldownText(cooldownText);
        }
        else
        {
            _view.SetSkillCooldownText("");
        }
    }

    private void SetButton()
    {
        Action[] changePartyMonsterActions = new Action[_view.PartyMonsterBtns.Length];
        for (int i = 0; i < changePartyMonsterActions.Length; i++)
        {
            int index = i;
            changePartyMonsterActions[i] = () => OnChangePartyMonster(index);
        }

        _view.SetButton(OnPause, OnSkill, OnActionMode, changePartyMonsterActions);
    }

    private void OnActionMode()
    {
        Monster.ActionModeHandler.ChangeActionMode();

        bool isCombat = Monster.ActionModeHandler.CurrentMode == Define.ActionMode.Combat;

        _view.SetGameObject(isCombat);
    }

    public void OnChangePartyMonster(int index)
    {
        _currentIndex = index;
        _view.BattleManager.SwitchingSystem.SwitchPartnerMonster(index);
        SetMonsterAndActionModeIcon(index);

        // 기존 이벤트 핸들러 제거 (중복 등록 방지)
        Monster.HealthSystem.OnHPChangedEvent -= UpdataePartyHPBar;

        // 새 이벤트 핸들러 등록
        Monster.HealthSystem.OnHPChangedEvent += UpdataePartyHPBar;

        _view.HighlightCurrentMonsterButton(index);
        SetSkillCooldownText();
        _view.SetSkillButtonSprite(SetSpriteSkillBtn(index));
    }

    private void SetMonsterAndActionModeIcon(int index)
    {
        Monster = _baseMonsters[index];
        MonsterAIHandler = _partnerAIHandlers[index];
        _view.SetGameObject(false);
    }

    private void OnSkill()
    {
        // 스킬 사용 가능한지 체크
        if (Monster.SkillHandler.IsCooldown)
        {
            return;
        }
        Monster.StateMachine.ChangeState(Monster.StateMachine.CastState);
    }

    private void OnPause()
    {
        UIManager.Instance.InstanciateUIPopup<UIPopup_PauseScreen>();
    }

    private void SetBar()
    {
        _view.SetPlayerHPBar(1);
    }

    public void UpdatePlayerHP(float currentHp, float maxHp)
    {
        float playerHP = currentHp / maxHp;
        _view.SetPlayerHPBar(playerHP);
    }

    private void UpdataePartyHPBar(float currentHp, float maxHp)
    {
        float monsterHpRatio = currentHp / maxHp;
        _view.SetPartyHPBar(_currentIndex, monsterHpRatio);
    }

    public void UpdatePartyHPBarCallBack()
    {
        UpdataePartyHPBar(Monster.HealthSystem.Current, Monster.HealthSystem.Max);
    }

    public void SetPartyMonsterUI(int count)
    {
        for (int i = 0; i < _view.PartyHPImage.Length; i++)
        {
            _view.PartyHPImage[i].gameObject.SetActive(i < count);
        }
    }

    public IEnumerator UpdateCooldownImages()
    {
        while (true)
        {
            for (int i = 0; i < _monsterSkillHandlers.Count; i++)
            {
                var skillHandler = _monsterSkillHandlers[i];
                float remainingCooldown = skillHandler.RemainingCooldown;
                float fillAmount = remainingCooldown / skillHandler.Info.CoolDown;
                _view.SetPartyCooldownImage(i, fillAmount);
            }
            _view.SkillButtonOnOff();
            yield return new WaitForSeconds(_updateInterval);
        }
    }
}

public interface IScene
{
    void RemovePartyMonsterButtonClickListener(int index);
    void HighlightCurrentMonsterButton(int currentIndex);
    void OnChangePartyMonster(int currentindex);
}
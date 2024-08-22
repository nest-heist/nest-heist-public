using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class UISubItem_MonsterButton : UISubItem
{
    private Button _monsterBtn;
    private UserMonster _currentMonster;
    public Image MonsterImage;
    public Image IndexImage;
    public Sprite[] IndexSprtieList;
    public TextMeshProUGUI LevelText;

    protected override void Awake()
    {
        base.Awake();
        _monsterBtn = GetComponent<Button>();
    }

    public void Initialize(UserMonster userMonster, Action onClickAction)
    {
        _currentMonster = userMonster;
        Sprite monsterSprite = ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", _currentMonster.AllStatData.BaseStat.MonsterId);
        MonsterImage.sprite = monsterSprite;
        LevelText.text = $"Lv {userMonster.UserData.Level}";
        StartCoroutine(AdjustImageSize(monsterSprite));

        _monsterBtn.onClick.AddListener(() => { onClickAction(); OnUIClickSound(); });
    }

    private IEnumerator AdjustImageSize(Sprite monsterSprite)
    {
        // 한 프레임 대기
        yield return new WaitForEndOfFrame();

        var rect = GetComponent<RectTransform>().rect;
        var width = monsterSprite.textureRect.width;
        var scale = rect.width / width;
        MonsterImage.rectTransform.sizeDelta = new Vector2(width * scale, monsterSprite.textureRect.height * scale);
    }

    public UserMonster GetCurrentMonster()
    {
        return _currentMonster;
    }

    private void SetDarken(bool isDarkened)
    {
        Color color = MonsterImage.color;
        color.a = isDarkened ? 0.5f : 1.0f; // 투명도를 조절하여 어둡게 표현
        MonsterImage.color = color;
    }
    public void SetIndex(int index)
    {
        if (index >= 0)
        {
            IndexImage.sprite = IndexSprtieList[index];
            IndexImage.gameObject.SetActive(true);
            SetDarken(true);
        }
        else
        {
            IndexImage.sprite = null;
            IndexImage.gameObject.SetActive(false);
            SetDarken(false);
        }
    }
}
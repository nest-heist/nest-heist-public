using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISubItem_GrowUpMonsterButton : MonoBehaviour
{
    public UserMonster CurrentMonster;
    public UserMonster LevelUpMonster;
    public UISubItem_GrowUpMonsterButton_Presenter Presenter;
    public UIPopup_GrowUpScreen GrowUpScreen;
    public Image MonsterImage;
    public Button GrowUpMonster;
    public Toggle CheckBox;

    private string selectedExpMonsterId;

    private void Awake()
    {
        Presenter = new UISubItem_GrowUpMonsterButton_Presenter(this);
        Transform parent = transform.parent;
        while (parent != null)
        {
            GrowUpScreen = parent.GetComponent<UIPopup_GrowUpScreen>();
            if (GrowUpScreen != null)
            {
                // 부모를 찾으면 루프 종료
                break;
            }
            // 없으면 다음 부모로
            parent = parent.parent;
        }
        LevelUpMonster = GrowUpScreen.Presenter.CurrentMonster;
    }

    private void Start()
    {
        Presenter.Init();
    }

    public void SetMonster(UserMonster userMonster)
    {
        CurrentMonster = userMonster;
    }

    public void SetImage(Sprite sprite)
    {
        MonsterImage.sprite = sprite;
        StartCoroutine(AdjustImageSize(sprite));
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

    public void SetButton(Action onGrowUp)
    {
        GrowUpMonster.onClick.AddListener(() =>
        {
            if (GameManager.Instance.UserMonsterSystem.PartyUserMonsterList.Any(um => um.UserData.Id == CurrentMonster.UserData.Id))
            {
                UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
                popup.SetText("파티에 등록된 몬스터는 성장 재료로 사용할 수 없습니다.");
            }
            else
            {
                onGrowUp(); OnClickUISound();
            }
        });
    }

    private void OnClickUISound()
    {
        AudioManager.Instance.PlaySFX("UIClick");
    }

    public void SelectExpMonster(string monsterId)
    {
        selectedExpMonsterId = monsterId;
    }

    public string GetSelectedExpMonsterId()
    {
        return selectedExpMonsterId;
    }

    // 체크박스 상태 반환 메서드
    public bool IsChecked()
    {
        return CheckBox.isOn;
    }

    public void SetChecked(bool isChecked)
    {
        CheckBox.isOn = isChecked;
    }

    public void DisableCheckBox()
    {
        CheckBox.interactable = false;
    }
}

public class UISubItem_GrowUpMonsterButton_Presenter
{
    private UISubItem_GrowUpMonsterButton _view;

    public UISubItem_GrowUpMonsterButton_Presenter(UISubItem_GrowUpMonsterButton view)
    {
        _view = view;
    }

    public void Init()
    {
        _view.SetButton(OnGrowUpMonster);
        _view.SelectExpMonster(_view.CurrentMonster.UserData.Id);
    }

    public void SetSubItem(UserMonster userMonster)
    {
        _view.SetMonster(userMonster);
        Sprite monsterSprite = ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", _view.CurrentMonster.AllStatData.BaseStat.MonsterId);
        _view.SetImage(monsterSprite);
    }

    private void OnGrowUpMonster()
    {
        bool isChecked = !_view.CheckBox.isOn;
        _view.SetChecked(isChecked);

        int currentExp = _view.GrowUpScreen.Presenter.GetCurrentTotalExp();
        int maxExp = DataManager.Instance.ReadOnlyDataSystem.MonsterLevelUp[_view.GrowUpScreen.Presenter.CurrentMonster.UserData.Level].SumOfExp;

        int monsterExpValue = 1;

        if (isChecked)
        {
            if (currentExp + monsterExpValue > maxExp)
            {
                // 현재 경험치에 몬스터의 경험치를 더했을 때 최대 경험치를 초과하면 체크박스를 해제하고 비활성화
                _view.SetChecked(false);
                _view.DisableCheckBox();
            }
            else
            {
                // 최대 경험치를 초과하지 않으면 체크박스를 활성화하고 경험치를 업데이트
                _view.GrowUpScreen.Presenter.AddExp(monsterExpValue);
                _view.GrowUpScreen.Presenter.AddDisplayExp(monsterExpValue);
                _view.GrowUpScreen.ExpCount += monsterExpValue;
                _view.GrowUpScreen.Presenter.UpdateExpBar();
                _view.GrowUpScreen.UpdateExpCountText();
            }
        }
        else
        {
            // 체크박스를 해제할 때 경험치를 다시 차감
            _view.GrowUpScreen.Presenter.AddExp(-monsterExpValue);
            _view.GrowUpScreen.Presenter.AddDisplayExp(-monsterExpValue);
            _view.GrowUpScreen.ExpCount -= monsterExpValue;
            _view.GrowUpScreen.Presenter.UpdateExpBar();
            _view.GrowUpScreen.UpdateExpCountText();
        }
    }
}
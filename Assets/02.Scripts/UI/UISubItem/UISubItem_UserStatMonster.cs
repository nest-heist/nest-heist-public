using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISubItem_UserStatMonster : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI MonsterNameText;
    public TextMeshProUGUI MonsterLevelText;

    public TextMeshProUGUI MonsterSkillNameText;
    public TextMeshProUGUI MonsterHPText;
    public TextMeshProUGUI MonsterATKText;
    public TextMeshProUGUI MonsterDefenceText;
    public TextMeshProUGUI MonsterCriticalText;
    public TextMeshProUGUI MonsterCriticalDamageText;

    [Header("Image")]
    public Image MonsterImage;

    public UISubItem_UserStatMonster_Presenter Presenter;

    public void Init(UserMonster partnerMonster)
    {
        Presenter = new UISubItem_UserStatMonster_Presenter(this, partnerMonster);
    }

    public void SetText(string monsterName, string monsterLevel, string monsterSkillName, string monsterHP, string monsterATK, string monsterDefence, string monsterCritical, string monsterCriticalDamage)
    {
        MonsterNameText.text = monsterName;
        MonsterLevelText.text = $"Lv {monsterLevel}";
        MonsterSkillNameText.text = monsterSkillName;
        MonsterHPText.text = monsterHP;
        MonsterATKText.text = monsterATK;
        MonsterDefenceText.text = monsterDefence;
        MonsterCriticalText.text = monsterCritical;
        MonsterCriticalDamageText.text = monsterCriticalDamage;
    }

    public void SetSprite(Sprite monsterImage)
    {
        MonsterImage.sprite = monsterImage;
    }
}

public class UISubItem_UserStatMonster_Presenter
{
    private UISubItem_UserStatMonster _view;
    private UserMonster _partnerMonster;

    public UISubItem_UserStatMonster_Presenter(UISubItem_UserStatMonster view, UserMonster partnerMonster)
    {
        _view = view;

        _partnerMonster = partnerMonster;

        SetText();
        SetSprite();
    }

    private void SetText()
    {
        MonsterInfoData monsterInfo = DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[_partnerMonster.UserData.MonsterId];
        MonsterSkillInfoData skillInfo = DataManager.Instance.ReadOnlyDataSystem.MonsterSkillInfo[_partnerMonster.UserData.SkillId];

        string monsterName = monsterInfo.Name;
        string monsterLevel = _partnerMonster.UserData.Level.ToString();
        string monsterSkillName = skillInfo.Name;
        string monsterHP = _partnerMonster.CurrentStat.MaxHP.ToString();
        string monsterATK = _partnerMonster.CurrentStat.Attack.ToString();
        string monsterDefence = _partnerMonster.CurrentStat.Defence.ToString();
        string monsterCritical = ((int)_partnerMonster.CurrentStat.Critical).ToString();
        string monsterCriticalDamage = _partnerMonster.CurrentStat.CriticalDamage.ToString();

        _view.SetText(monsterName, monsterLevel, monsterSkillName, monsterHP, monsterATK, monsterDefence, monsterCritical, monsterCriticalDamage);
    }

    private void SetSprite()
    {
        Sprite monsterSprite = ResourceManager.Instance.LoadSprite("Icon/MonsterIcon/monster-icon", _partnerMonster.UserData.MonsterId);

        _view.SetSprite(monsterSprite);
    }
}
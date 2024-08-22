using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopup_Setting : UIPopup
{
    [Header("Button")]
    public Button ExitButton;
    public Button ExitBackground;
    public Button SignOutButton;
    public Button ExitGameButton;

    [Header("Slider")]
    public Slider BGMSlider;
    public Slider SFXSlider;

    private UIPopup_Setting_Presenter _presenter;

    protected override void Start()
    {
        base.Start();

        _presenter = new UIPopup_Setting_Presenter(this);
    }

    public void SetButton(Action OnExit)
    {
        ExitButton.onClick.AddListener(() => OnExit());
    }

    public void SetSliderChange(Action BGM, Action SFX)
    {
        AddPointerUpEvent(BGMSlider, BGM);
        AddPointerUpEvent(SFXSlider, SFX);
    }

    public void SetSliderValue(float bgm, float sfx)
    {
        BGMSlider.value = bgm;
        SFXSlider.value = sfx;
    }

    private void AddPointerUpEvent(Slider slider, Action action)
    {
        EventTrigger trigger = slider.gameObject.GetOrAddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { action?.Invoke(); });
        trigger.triggers.Add(entry);
    }
}

public class UIPopup_Setting_Presenter : Presenter
{
    private UIPopup_Setting _view;
    private LocalSoundSettingData _settingData;

    public UIPopup_Setting_Presenter(UIPopup_Setting view)
    {
        _view = view;

        SetButton();
        SetSliderValueFromJson();
        SetSliderChange();
    }

    private void SetSliderValueFromJson()
    {
        _settingData = DataManager.Instance.LocalDataSystem.SoundSetting;
        _view.SetSliderValue(_settingData.BGMVolume, _settingData.SFXVolume);
    }

    private void SetSliderChange()
    {
        _view.SetSliderChange(BGM, SFX);
    }

    public void SFX()
    {
        _settingData.SFXVolume = _view.SFXSlider.value;

        EventManager.Instance.Invoke(EventType.OnSetVolume, this, _settingData);
    }

    public void BGM()
    {
        _settingData.BGMVolume = _view.BGMSlider.value;

        EventManager.Instance.Invoke(EventType.OnSetVolume, this, _settingData);
    }

    private void SetButton()
    {
        _view.ExitBackground.onClick.AddListener(() => { UIManager.Instance.DestoryUIPopup(this._view); });
        _view.ExitGameButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 모드 종료
#else
            Application.Quit(); // 빌드된 게임에서 종료
#endif
        });

        _view.SignOutButton.onClick.AddListener(() =>
        {
            FirebaseManager.Instance.AuthSystem.SignOut();
        });

        _view.SetButton(() =>
        {
            UIManager.Instance.DestoryUIPopup(this._view);
        });
    }

}

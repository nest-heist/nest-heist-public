using System;

/// <summary>
/// 사운드 세팅 데이터
/// </summary>
[Serializable]
public class LocalSoundSettingData
{
    // 0~1 사이의 값으로 저장
    public float BGMVolume;
    public float SFXVolume;

    public LocalSoundSettingData()
    {
        BGMVolume = 0.01f;
        SFXVolume = 0.01f;
    }
}

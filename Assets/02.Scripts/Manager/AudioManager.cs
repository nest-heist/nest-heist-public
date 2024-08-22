using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityAudioSource;
using System.Collections;
using static Define;

public enum SoundType
{
    Bgm,
    Sfx,
    COUNT
}

public class AudioManager : Singleton<AudioManager>, IListener
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] private AudioClip[] _sfxClips;

    private Dictionary<string, AudioClip> _bgmDic = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDic = new Dictionary<string, AudioClip>();

    [SerializeField] AudioSource _bgmPlayer = null;
    [SerializeField] AudioSource[] _sfxPlayer = null;
    int Channels = 32;

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    /// <summary>
    /// </summary>
    protected override void Start()
    {
        base.Start();

        SetDictionary();
        _sfxPlayer = new AudioSource[Channels];
        CreateAudioPlayer();
    }

    /// <summary>
    /// 세이브 된 세팅 파일 LocalData에서 불러오면 볼륨 조절
    /// </summary>
    public void Init()
    {
        // 이벤트 -= += 로 두 번 등록되는 것을 방지
        EventManager.Instance.RemoveListener(EventType.OnSetVolume, this);
        EventManager.Instance.AddListener(EventType.OnSetVolume, this);
    }

    private void SetDictionary()
    {
        for (int i = 0; i < _bgmClips.Length; i++)
        {
            _bgmDic.Add(_bgmClips[i].name, _bgmClips[i]);
        }
        for (int i = 0; i < _sfxClips.Length; i++)
        {
            _sfxDic.Add(_sfxClips[i].name, _sfxClips[i]);
        }
    }

    public void PlayBGM(string bgmName)
    {
        try
        {
            AudioClip clip = _bgmDic[bgmName];

            if (clip == null)
            {
                throw new System.NullReferenceException($"BGM Clip Does not Exist");
            }
            _bgmPlayer.clip = clip;
            _bgmPlayer.Play();
            return;
        }
        catch (System.NullReferenceException e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
        catch (System.Exception e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    public void StopBgm()
    {
        _bgmPlayer?.Stop();
    }

    public void PlaySFX(string sfxName)
    {
        try
        {
            AudioClip clip = _sfxDic[sfxName];

            if (clip == null)
            {
                throw new System.NullReferenceException($"SFX Clip Does not Exist");
            }

            for (int i = 0; i < _sfxPlayer.Length; i++)
            {
                // SFXPlayer에서 재생 중이지 않은 Audio Source를 발견했다면 
                if (!_sfxPlayer[i].isPlaying)
                {
                    _sfxPlayer[i].clip = clip;
                    _sfxPlayer[i].Play();
                    return;
                }
            }
            return;
        }
        catch(System.NullReferenceException ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
        catch(System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }

    }

    /// <summary>
    /// 오디오를 미리 풀링해서 생성해놓고 초기화 까지 완료
    /// </summary>
    private void CreateAudioPlayer()
    {
        GameObject obj = new GameObject("BGMPlayer");
        obj.transform.SetParent(transform);
        AudioSource bgmPlayer = obj.AddComponent<AudioSource>();
        _bgmPlayer = bgmPlayer;
        _bgmPlayer.loop = true;
        _bgmPlayer.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Bgm")[0];
        _bgmPlayer.playOnAwake = false;


        for (int i = 0; i < Channels; i++)
        {
            GameObject obj2 = new GameObject("SfxPlayer");
            obj2.transform.SetParent(transform);
            AudioSource SfxPlayer = obj2.AddComponent<AudioSource>();
            _sfxPlayer[i] = SfxPlayer;
            _sfxPlayer[i].loop = false;
            _sfxPlayer[i].playOnAwake = false;
            _sfxPlayer[i].outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Sfx")[0];
        }
    }

    /// <summary>
    /// 볼륨의 크기를 조절 오디오 믹서를 이용
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void SetVolume(SoundType type, float value)
    {
        _audioMixer.SetFloat(type.ToString(), value);
    }

    /// <summary>
    /// 오디오 매니저가 먼저 초기화, 생성 되어서 이벤트 매니저에다가 일단 등록하고 LocalData의 값을 받아서 볼륨을 조절
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="sender"></param>
    /// <param name="paramObjects"></param>
    public void OnEvent(EventType eventType, object sender, params object[] paramObjects)
    {
        if (eventType == EventType.OnSetVolume)
        {
            // 받아온 사운드 설정 데이터 저장
            LocalSoundSettingData settingData = paramObjects[0] as LocalSoundSettingData;
            try
            {
                if (settingData == null)
                {
                    throw new System.NullReferenceException($"AudioManager::OnEvent() LocalSoundSettingData is null");
                }
            }
            catch(System.NullReferenceException e)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string uid = DataManager.Instance.ServerDataSystem.User.UserId;
                FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName, settingData);
                Logging.LogError(e.Message);
                return;
            }

            DataManager.Instance.LocalDataSystem.SaveJsonData(settingData);

            // 볼륨 조절
            SetVolume(SoundType.Bgm, Mathf.Log10(settingData.BGMVolume) * 20);
            SetVolume(SoundType.Sfx, Mathf.Log10(settingData.SFXVolume) * 20);
        }
    }
}


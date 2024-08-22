using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 로컬에 Json파일로 데이터를 저장하고 불러오는 클래스
/// </summary>
public class LocalDataSystem
{
    string _persistentDataPath;

    public LocalSoundSettingData SoundSetting { get; private set; }
    public LocalPageTutorialData PageTutorial { get; private set; }
    public LocalClickTutorialData ClickTutorial { get; private set; }

    public void Init()
    {
        _persistentDataPath = Application.persistentDataPath;

        LoadAllData();
        InitAudioManager();
    }

    private void LoadAllData()
    {
        SoundSetting = LoadJsonData<LocalSoundSettingData>();
        PageTutorial = LoadJsonData<LocalPageTutorialData>();
        ClickTutorial = LoadJsonData<LocalClickTutorialData>();
    }

    /// <summary>
    /// 사운드 크기 조절, 데이터 변경으로 이벤트 리스너 설정
    /// </summary>
    private void InitAudioManager()
    {
        AudioManager.Instance.Init();
        EventManager.Instance.Invoke(EventType.OnSetVolume, this, SoundSetting);
    }
    
    /// <summary>
    /// 로컬에 json파일로 저장하기 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataClass"></param>
    public void SaveJsonData<T>(T dataClass)
    {
        string path = _persistentDataPath + $"/{typeof(T).ToString()}.json";
        File.WriteAllText(path, JsonUtility.ToJson(dataClass));
        Logging.Log($"LocalDataSystem::SaveJsonData : {path}");
    }

    /// <summary>
    /// 로컬에 저장된 json파일 불러오기
    /// 없으면 new()로 생성한다. Data클래스는 MonoBehaviour를 상속받지 않는 클래스만 사용 가능
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadJsonData<T>() where T : new()
    {
        try
        {
            string path = _persistentDataPath + $"/{typeof(T).ToString()}.json";
            if (!File.Exists(path))
            {
                Logging.Log($"LocalDataSystem::LoadJsonData : {typeof(T).ToString()}.json not found.");

                T localTData = new T();

                return localTData;
            }

            string jsonData = File.ReadAllText(path);
            Logging.Log($"DataManager::LoadJsonData : {path} loaded.");
            return JsonUtility.FromJson<T>(jsonData);
        }
        catch (Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
            return default(T);
        }
    }
}
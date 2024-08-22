using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

/// <summary>
/// ResourceManger.Instance.Load<T>() 
/// 여러 번 리소스가 로드하지 않도록 캐싱한다. 
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    public Dictionary<string, object> AssetPools = new Dictionary<string, object>();

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    /// <summary>
    /// 로드 할 때 캐싱된 리소스가 있으면 리턴하고 없으면 로드한다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        try
        {
            if (AssetPools.ContainsKey(path))
            {
                return AssetPools[path] as T;
            }
            else
            {
                T asset = Resources.Load<T>(path);
                if (asset == null)
                {
                    throw new System.NullReferenceException();
                }

                AssetPools.Add(path, asset);
                return asset;
            }
        }
        catch (System.NullReferenceException ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"Asset is Null{ex.Message}", ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
            return null;
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Prefabs/ 뒤 경로부터 패스로 넣어주기
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        try
        {
            if (prefab == null)
            {
                throw new System.NullReferenceException();
            }
        }
        catch (System.NullReferenceException e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName, prefab);
            Logging.LogError(e.Message);
        }
        return Instantiate(prefab, parent);
    }

    /// <summary>
    /// 프리팹 이름하고 동일하게 이름을 설정하고 생성하기
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        return go;
    }

    /// <summary>
    /// 특정 스프라이트 시트에서 ID에 해당하는 스프라이트를 로드하여 캐싱합니다.
    /// </summary>
    /// <param name="spriteSheetPath"></param>
    /// <param name="spriteID"></param>
    /// <returns></returns>
    public Sprite LoadSprite(string spriteSheetPath, string spriteID)
    {
        try
        {
            string cacheKey = $"{spriteSheetPath}/{spriteID}";

            if (AssetPools.ContainsKey(cacheKey))
            {
                return AssetPools[cacheKey] as Sprite;
            }

            Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheetPath);
            if (sprites.Length == 0)
            {
                throw new System.NullReferenceException($"ResourceManager::LoadSprite() Failed to load sprite sheet: {spriteSheetPath}");
            }

            foreach (Sprite sprite in sprites)
            {
                string spriteKey = $"{spriteSheetPath}/{sprite.name}";
                if (!AssetPools.ContainsKey(spriteKey))
                {
                    AssetPools.Add(spriteKey, sprite);
                }
            }

            if (AssetPools.ContainsKey(cacheKey))
            {
                return AssetPools[cacheKey] as Sprite;
            }
            else
            {
                throw new System.NullReferenceException($"ResourceManager::LoadSprite() Sprite with ID {spriteID} not found in sheet: {spriteSheetPath}");
            }
        }
        catch (System.NullReferenceException e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName, AssetPools);
            Logging.LogError(e.Message);
            return null;
        }
        catch (System.Exception e)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName);
            Logging.LogError(e.Message);
            return null;
        }
    }
}


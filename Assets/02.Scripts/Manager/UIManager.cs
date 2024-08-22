using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// UIPopup 스택으로 관리한다 
/// </summary>
public class UIManager : Singleton<UIManager>
{
    int _sortingOrder = -20;

    Stack<UIPopup> _popupStack = new Stack<UIPopup>();

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();
    }

    /// <summary>
    /// UI들 동적으로 만들 때 루트로 사용할 GameObject
    /// </summary>
    public GameObject UIRoot
    {
        get
        {
            GameObject uiroot = GameObject.Find("UIRoot");
            if (uiroot == null)
            {
                uiroot = new GameObject { name = "UIRoot" };
            }

            return uiroot;
        }
    }

    /// <summary>
    /// 씬마다 UIScene은 하나만 있다.
    /// 여기 위에다가 UI들을 올린다
    /// </summary>
    public UIScene UIScene { get; private set; }

    /// <summary>
    /// UIScene은 -25 sortingOrder이다.
    /// </summary>
    /// <param name="go"></param>
    public void SetUIScene(GameObject go, bool isNotOverlay = false)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        if (!isNotOverlay) canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = -25;

        UIScene uiScene = go.GetOrAddComponent<UIScene>();
        UIScene = uiScene;

        go.transform.SetParent(UIRoot.transform);
    }

    /// <summary>
    /// 동적생성 - UISubItem
    /// Resources/Prefabs/UI/SubItem/에 있는 프리팹을 로드해서 만든다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public T InstanciateSubItem<T>(Transform parent = null, string name = null) where T : UISubItem
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject prefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/UI/UISubItem/{name}");

        GameObject go = Instantiate(prefab);
        if (parent != null)
        {
            go.transform.SetParent(parent);
        }

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;

        return go.GetOrAddComponent<T>();
    }

    /// <summary>
    /// 동적생성 - UIScene
    /// Resources/Prefabs/UI/Scene/ 에 있는 프리팹을 로드해서 만든다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T InstanciateUIScene<T>(string name = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/UIScene/{name}");

        T sceneUI = go.GetOrAddComponent<T>();

        go.transform.SetParent(UIRoot.transform);

        return sceneUI;
    }


    #region UIPopup

    /// <summary>
    /// 동적생성 - UIPopup
    /// Resources/Prefabs/UI/Popup/ 에 있는 프리팹을 로드해서 만든다
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public T InstanciateUIPopup<T>(string name = null, Transform parent = null) where T : UIPopup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject go = ResourceManager.Instance.Instantiate($"UI/UIPopup/{name}");
        T popup = go.GetOrAddComponent<T>();
        _popupStack.Push(popup);

        if (parent != null)
        {
            go.transform.SetParent(parent);
        }
        else
        {
            go.transform.SetParent(UIRoot.transform);
        }

        go.transform.localScale = Vector3.one;

        return popup;
    }

    /// <summary>
    /// 해당하는 캔버스 sortingOrder를 설정한다
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sort"></param>
    public void SetUIPopup(GameObject go, bool sort = true)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _sortingOrder;
            _sortingOrder++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// 팝업 끄기 
    /// 가장 위에 있는 것만 지울 수 있다 
    /// </summary>
    /// <param name="popup"></param>
    public void DestoryUIPopup(UIPopup popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        // 맨 위 팝업만 지울 수 있다
        if (_popupStack.Peek() != popup)
        {
            Logging.LogError("Close Popup Failed!");
            return;
        }

        _popupStack.Pop();

        // 팝업이 널인데 스택에 남아있을 때
        if (popup == null)
        {
            _sortingOrder--;
            return;
        }

        // 팝업스크립트 붙어있는 오브젝트가 삭제되었을 때
        if (popup != null && popup.gameObject == null)
        {
            popup = null;
            _sortingOrder--;
            return;
        }

        // 그냥 삭제하는 일반적인 케이스
        if (popup != null && popup.gameObject != null)
        {
            Destroy(popup.gameObject);
            popup = null;
            _sortingOrder--;
            return;
        }
    }

    /// <summary>
    /// 팝업 다 끄기
    /// </summary>
    public void DestoryAllUIPopup()
    {
        while (_popupStack.Count > 0)
        {
            UIPopup popup = _popupStack.Pop();

            // 씬 넘어가는데 팝업 삭제 안하고 씬 넘겼을 때
            if(popup == null)
            {
                _sortingOrder--;
                continue;
            }
            
            // 팝업스크립트 붙어있는 오브젝트가 삭제되었을 때
            if(popup != null && popup.gameObject == null)
            {
                popup = null;
                _sortingOrder--;
                continue;
            }

            // 일반적으로 팝업 다 삭제하려고 할 떄
            if (popup != null && popup.gameObject != null)
            {
                Destroy(popup.gameObject);
                popup = null;
                _sortingOrder--;
                continue;
            }
        }
    }

    /// <summary>
    /// 현재 스택에 있는 팝업을 찾아서 리턴한다
    /// UIManager.Instance.FindUIPopup<UIPopup_Play>()?.RefreshHpBar(); 이렇게 사용
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T FindUIPopup<T>() where T : UIPopup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    #endregion
}

using UnityEngine;
using UnityEngine.UI;

public class UISubItem_DungeonSelectButton : MonoBehaviour
{
    public Text stageText;
    public GameObject LockPanel;
    private string _dungeonId;
    private Button _selectStageBtn;
    private UIPopup_DungeonSelectStage _stageSelectPresenter;
    private UIPopup_DungeonEnterCheck _enterCheck;
    private bool _isUnlocked;

    private void Awake()
    {
        _selectStageBtn = GetComponent<Button>();
        _selectStageBtn.onClick.AddListener(OnButtonClicked);
        _selectStageBtn.onClick.AddListener(() => OnUIClickSound());

        // 부모 계층을 탐색하여 DungeonSelectStagePresenter 할당
        Transform parent = transform.parent;
        while (parent != null)
        {
            _stageSelectPresenter = parent.GetComponent<UIPopup_DungeonSelectStage>();
            if (_stageSelectPresenter != null)
            {
                // 부모를 찾으면 루프 종료
                break;
            }
            // 없으면 다음 부모로
            parent = parent.parent;
        }
    }

    private void OnUIClickSound()
    {
        AudioManager.Instance.PlaySFX("UIClick");
    }

    public void Initialize(DungeonInfoData dungeonInfo, bool isUnlocked)
    {
        _dungeonId = dungeonInfo.Id;
        stageText.text = FormatStageText(dungeonInfo.Stage);

        // _selectStageBtn.interactable = isUnlocked;
        _isUnlocked = isUnlocked;
        LockPanel.SetActive(!isUnlocked);
    }

    private string FormatStageText(int stage)
    {
        int major = stage / 10;
        int minor = stage % 10;
        return $"{major}-{minor}";
    }

    private void OnButtonClicked()
    {
        if (_isUnlocked)
        {
            _stageSelectPresenter.CurrentDungeonId = _dungeonId;
            GameManager.Instance.DungeonStageSystem.SetDungeonId(_dungeonId);
            UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonEnterCheck>();
            _enterCheck = FindObjectOfType<UIPopup_DungeonEnterCheck>();
            _enterCheck.DungeonId = _dungeonId;
        }
        else
        {
            UIPopup_Warning popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Warning>();
            popup.SetText("이전 스테이지를 클리어 시 잠금이 해제됩니다.");
        }
    }
}

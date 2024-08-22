using TMPro;
using UnityEngine;

public class ExitTeleport : MonoBehaviour
{
    private LayerMask _playerMask;
    private Teleport _linkTeleport;
    public bool IsActive = true;
    private UIPopup_DungeonRewardScreen _screen;

    private void Awake()
    {
        _playerMask = LayerMask.GetMask("Player");
    }

    public void Init(Teleport linkTeleport)
    {
        _linkTeleport = linkTeleport;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActive && (1 << other.gameObject.layer & _playerMask) != 0)
        {
            Time.timeScale = 0f;
            DungeonBattleManager.Instance.PhaseSystem.ExitCurrentPhase();
            UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonRewardScreen>();
            _screen = FindObjectOfType<UIPopup_DungeonRewardScreen>();
            _screen.UpExpCount = DungeonBattleManager.Instance.InfoData.ClearExp;
            _screen.Init(DungeonBattleManager.Instance.DropSystem);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer & _playerMask) != 0)
        {
            IsActive = true;
        }
    }
}

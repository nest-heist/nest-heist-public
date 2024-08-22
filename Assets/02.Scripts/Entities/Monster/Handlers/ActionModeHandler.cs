using System;

/// <summary>
/// 현재 액션 상태를 다루고 다음 액션으로 넘어가게 하는 핸들러
/// </summary>
public class ActionModeHandler
{
    private Define.ActionMode _curMode;
    public event Action OnChangedMode;

    public Define.ActionMode CurrentMode => _curMode;

    public ActionModeHandler()
    {
        _curMode = Define.ActionMode.NonCombat;
    }

    public Define.ActionMode GetActionMode()
    {
        return _curMode;
    }
    public void CallModeChangeEvent()
    {
        OnChangedMode?.Invoke();
    }
    public void ChangeActionMode()
    {
        _curMode += 1;
        if (_curMode == Define.ActionMode.Count)
        {
            _curMode = 0;
            CallModeChangeEvent();
            return;
        }
        CallModeChangeEvent();
        return;
    }
}
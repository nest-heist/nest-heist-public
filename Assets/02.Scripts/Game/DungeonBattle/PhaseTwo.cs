using System;
using System.Collections;
using UnityEngine;

public class PhaseTwo : IDungeonPhase
{
    private DungeonBattleManager _battleManager;
    public string CatchEggId = null;

    public PhaseTwo(DungeonBattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    public event Action<string> OnPhaseChangeRequested;

    public void EnterPhase()
    {
        _battleManager.PhaseSystem.GenerateExitTeleport();
        _battleManager.PhaseCount = 2;
    }

    public void ExitPhase()
    {
        CatchEggId = _battleManager.GetEggIdSet;

        if (CatchEggId == null) return;

        ServerUserEggData newEggData = new ServerUserEggData
        {
            // 새로운 아이디를 임의로 구현
            Id = Guid.NewGuid().ToString(),
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            EggId = CatchEggId,
            AcquiredAt = "",
            HatchStart = "",
            IsHatched = false
        };

        DungeonBattleManager.Instance.DropSystem.TempEggDataList.Add(newEggData);
        DungeonBattleManager.Instance.DropSystem.ItemAndEggDataOnServer();
        _battleManager.PhaseCount = 0;
    }

    public void UpdatePhase()
    {
        // Debug.Log("페이즈 1 전환");
    }
}
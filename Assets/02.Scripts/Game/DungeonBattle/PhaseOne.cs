using System;
using UnityEngine;

public class PhaseOne : IDungeonPhase
{
    private DungeonBattleManager _battleManager;

    public PhaseOne(DungeonBattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    public event Action<string> OnPhaseChangeRequested;

    void IDungeonPhase.EnterPhase()
    {
        // Debug.Log("Phase One 시작");
    }

    void IDungeonPhase.ExitPhase()
    {
        // Debug.Log("Phase One 종료");
    }

    void IDungeonPhase.UpdatePhase()
    {
        // Debug.Log("Phase One 현황");
    }
}
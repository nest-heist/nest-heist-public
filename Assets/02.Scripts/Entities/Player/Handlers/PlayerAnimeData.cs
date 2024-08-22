using System;
using UnityEngine;

[Serializable]
public class PlayerAnimeData
{
    // 기본 상태
    [SerializeField] string _defaultParameterName = "@Default";
    [SerializeField] string _isIdleParameterName = "IsIdle";
    [SerializeField] string _isWalkParameterName = "IsWalk";

    // 알을 든 상태
    [SerializeField] string _carryParameterName = "@Carry";
    [SerializeField] string _isCarryIdleParameterName = "IsCarryIdle";
    [SerializeField] string _isCarryWalkParameterName = "IsCarryWalk";

    // 맞은 상태
    [SerializeField] string _isHitParameterName = "IsHit";

    // 죽은 상태
    [SerializeField] string _isDieParameterName = "IsDie";

    public int DefaultParameterHash { get; private set; }
    public int IsIdleParameterHash { get; private set; }
    public int IsWalkParameterHash { get; private set; }

    public int CarryParameterHash { get; private set; }
    public int IsCarryIdleParameterHash { get; private set; }
    public int IsCarryWalkParameterHash { get; private set; }

    public int IsHitParameterHash { get; private set; }
    public int IsDieParameterHash { get; private set; }

    public void Initialize()
    {
        DefaultParameterHash = Animator.StringToHash(_defaultParameterName);
        IsIdleParameterHash = Animator.StringToHash(_isIdleParameterName);
        IsWalkParameterHash = Animator.StringToHash(_isWalkParameterName);

        CarryParameterHash = Animator.StringToHash(_carryParameterName);
        IsCarryIdleParameterHash = Animator.StringToHash(_isCarryIdleParameterName);
        IsCarryWalkParameterHash = Animator.StringToHash(_isCarryWalkParameterName);

        IsHitParameterHash = Animator.StringToHash(_isHitParameterName);
        IsDieParameterHash = Animator.StringToHash(_isDieParameterName);
    }
}

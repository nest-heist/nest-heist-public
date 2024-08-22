using System;
using UnityEngine;

public abstract class BaseConditionSystem : MonoBehaviour, IConditionSystem
{
    protected int _current;
    public int Current { get => _current; }
    public abstract int Max { get; }
    public abstract ConditionType Type { get; }

    public event Action<int, int> OnModifyEvent;
    public event Action OnFilledEvent;
    public event Action OnLackedEvent;



    protected virtual void Start()
    {
        _current = Max;
        Modify(0);
    }

    public virtual bool Modify(int amount)
    {
        int prev = _current;
        int calCurrent = _current + amount;
        if (calCurrent > Max)
        {
            Fill();
            return false;
        }

        if (calCurrent < 0)
        {
            Lack();
            return false;
        }

        _current = calCurrent;

        if (prev != _current)
        {
            OnModifyEvent?.Invoke(_current, Max);
            return true;
        }

        return false;
    }
    protected virtual void Fill()
    {
        OnFilledEvent?.Invoke();
    }
    protected virtual void Lack()
    {
        OnLackedEvent?.Invoke();
    }
}
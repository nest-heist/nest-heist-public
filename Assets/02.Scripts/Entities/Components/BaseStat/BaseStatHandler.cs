using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class BaseStatHandler<T> : MonoBehaviour where T : IStat<T>
{
    [HideInInspector] public T CurrentStat { get; protected set; }
    [SerializeField] protected T _baseStat;
    protected List<T> _statModifiers = new List<T>();
    protected event Action OnStatChangedEvent;
    public string UniqueId;

    public void Init()
    {
        if (_baseStat == null) return;
        UpdateStat();
    }

    public virtual void UpdateStat()
    {
        CurrentStat = _baseStat.DeepCopy();

        _statModifiers.OrderBy(stat => stat.Type);

        foreach (T stat in _statModifiers)
        {
            switch (stat.Type)
            {
                case StatType.Add:
                    CurrentStat.Add(stat);
                    break;
                case StatType.Multiple:
                    CurrentStat.Multiply(stat);
                    break;
                case StatType.Override:
                    CurrentStat = stat.DeepCopy();
                    break;

            }
        }
        OnStatChanged();
    }
    public virtual void AddStat(T stat)
    {
        _statModifiers.Add(stat);
    }
    public virtual void RemoveStat(T stat)
    {
        _statModifiers.Remove(stat);
    }
    public virtual void OnStatChanged()
    {
        OnStatChangedEvent?.Invoke();
    }
    public virtual void SetBaseStat(T stat)
    {
        _baseStat = stat.DeepCopy();
    }
    public void AddStat(T stat, StatType type)
    {
        stat.Type = type;
        AddStat(stat);
    }
}

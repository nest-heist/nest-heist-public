using System;

public enum ConditionType
{
    Health,
    Mana,
    Stamina,
}

public interface IConditionSystem
{
    public ConditionType Type { get; }
    public int Current { get; }
    public int Max { get; }

    /// <summary>
    /// Occurs when the condition is modified.
    /// </summary>
    /// <param name="currentValue">The current value of the condition.</param>
    /// <param name="maxValue">The maximum value of the condition.</param>
    public event Action<int, int> OnModifyEvent;
    public bool Modify(int amount);
}
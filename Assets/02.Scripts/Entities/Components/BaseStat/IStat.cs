using System;

public interface IStat<T>
{
    public StatType Type { get; set; }
    public T DeepCopy();
    public void Add(T other);
    public void Multiply(T other);
    public T StatCalculator(Func<float, int, float> calculator, int num);
}

public enum StatType
{
    Add,
    Multiple,
    Override
}
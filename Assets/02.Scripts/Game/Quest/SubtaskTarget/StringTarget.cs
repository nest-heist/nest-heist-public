public class StringTarget : SubtaskTarget
{
    private string _value;
    public override object Value => _value;

    public StringTarget(string value)
    {
        _value = value;
    }

    public override bool IsEqual(object target)
    {
        if (target == null)
        {
            return false;
        }

        return _value == target as string;
    }
}
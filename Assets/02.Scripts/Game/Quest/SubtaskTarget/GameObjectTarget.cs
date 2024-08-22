using UnityEngine;

/// <summary>
/// 현재 사용 X
/// </summary>
public class GameObjectTarget : SubtaskTarget
{
    private GameObject _value;
    public override object Value => _value;

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool IsEqual(object target)
    {
        GameObject targetAsGameObject = target as GameObject;
        if (targetAsGameObject == null)
        {
            return false;
        }
        return targetAsGameObject.name.Contains(_value.name);
    }
}

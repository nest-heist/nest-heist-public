using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss001")]
public class DestroyAfterDeath : Action
{
    public float delay = 2.0f;

    public override TaskStatus OnUpdate()
    {
        GameObject.Destroy(gameObject, delay);
        UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonRewardScreen>();
        return TaskStatus.Success;
    }
}
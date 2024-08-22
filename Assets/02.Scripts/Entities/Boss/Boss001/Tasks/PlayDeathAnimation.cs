using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Boss001")]
public class PlayDeathAnimation : Action
{
    public override TaskStatus OnUpdate()
    {
        GetComponent<Animator>().SetTrigger("Die");
        return TaskStatus.Success;
    }
}

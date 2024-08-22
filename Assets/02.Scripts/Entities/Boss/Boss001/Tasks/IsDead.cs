using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Boss001")]
public class IsDead : Conditional
{
    private BossHealthSystem _healthSystem;

    public override void OnAwake()
    {
        _healthSystem = GetComponent<BossHealthSystem>();
    }

    public override TaskStatus OnUpdate()
    {
        return _healthSystem.IsDead() ? TaskStatus.Success : TaskStatus.Failure;
    }
}

using BehaviorDesigner.Runtime.Tasks;
public class IsCheckCooldown : Conditional
{
    public string CooldownTag;
    public float CoolTime;

    public override void OnAwake()
    {
        CooldownManager.Instance.StartCooldown(CooldownTag, CoolTime);
    }
    public override TaskStatus OnUpdate()
    {
        if (CooldownManager.Instance.IsCooldownActive(CooldownTag))
        {
            return TaskStatus.Failure;
        }
        CooldownManager.Instance.StartCooldown(CooldownTag, CoolTime);
        return TaskStatus.Success;
    }
}
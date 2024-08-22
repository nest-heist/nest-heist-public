public class PartnerMonster : BaseMonster
{
    protected override void Awake()
    {   
        base.Awake();
        IsPartner = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
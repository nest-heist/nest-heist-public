public struct StartHatchingBody
{
    public string Id;
    public string UserId;
    public string UserEggId;
    public string EggId;
    public ItemMaterial[] Materials;
    public int hoursToHatch;
}

public struct ItemMaterial
{
    public string ItemId;
    public int Quantity;
}

public struct HatchEggBody
{
    public string Id;
    public string UserId;
    public string MonsterId;
    public string SkillId;
}
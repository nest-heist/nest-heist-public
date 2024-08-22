public struct RewardBody
{
    public string UserId;
    public string EggId;
    public Item[] Items;
}

public struct Item
{
    public string ItemId;
    public int Quantity;
}

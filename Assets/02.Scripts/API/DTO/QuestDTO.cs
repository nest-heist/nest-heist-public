
public struct UpdateValueBody
{
    public string Id;
    public string QuestId;
    public string UserId;
    public int Value;
    public bool IsDailyQuest;
}

public struct AddCompletedBody
{
    public string Id;
    public string UserId;
    public bool IsDailyQuest;
}

public struct CompletedAndRewardBody
{
    public AddCompletedBody CompletedQuest;
    public RewardBody Reward;
    public LevelUpBody Exp;
}
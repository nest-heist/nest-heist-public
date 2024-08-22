
/// <summary>
/// 플레이어가 레벨업 할 때 적용
/// 플레이어 레벨 1업시마다 적용되는 수치
/// </summary>
public class PlayerLevelStatData
{
    // 플레이어 아이디
    public string PlayerId;

    // 플레이어 최대 체력
    public int MaxHP;

    // 플레이어 회피율
    public float Avoid;

    // 플레이어 이동 속도
    public float WalkSpeed;

    public PlayerStat ReturnStat()
    {
        PlayerStat newBase = new PlayerStat();
        newBase.MaxHP = MaxHP;
        newBase.Avoid = Avoid;
        newBase.WalkSpeed = WalkSpeed;

        return newBase;
    }
}
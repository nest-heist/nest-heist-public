
/// <summary>
/// 플레이어 기본 스텟
/// 플레이어 레벨 1일 때 적용되는 수치
/// </summary>
public class PlayerBaseStatData
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

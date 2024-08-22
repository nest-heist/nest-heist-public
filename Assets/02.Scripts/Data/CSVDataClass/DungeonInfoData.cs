using static Define;
/// <summary>
/// 던전 기본 정보
/// 각 입장하는 던전마다 부여되는 아이디
/// </summary>
public class DungeonInfoData
{
    // 던전 아이디
    public string Id;

    // 던전 환경 타입
    public EnvironmentType EnvironmentType;

    // 던전의 단계
    // 1-1(11)로 표현하지만 값은 int로 들고있다
    public int Stage;

    // 입장에 필요한 피로도
    public int Fatigue;

    // 들어갈 수 있는 최소 레벨
    public int MinLevel;

    // 들어갈 수 있는 최대 레벨
    public int MaxLevel;

    // 던전 너비
    public int DungeonWidth;

    // 던전 높이
    public int DungeonHeight;

    // 클리어시 경험치
    public int ClearExp;

    // 스폰되는 몬스터 수
    public int MonsterSpawnNum;
}

using System.Collections.Generic;
using static Define;
/// <summary>
/// 알 기본 정보
/// </summary>
public class EggInfoData
{
    // 알 아이디
    public string Id;

    // 알 이름 텍스트
    public string Name;

    // 태어날 몬스터 아이디
    public List<string> MonsterIdList;

    // 나올 몬스터의 타입
    // 알 색상 결정하기
    public EnvironmentType EnvironmentType;

    // 태어나는데 걸리는 시간 (분 단위)
    public int HatchTime;

    // 부화의 필요한 게이지
    public int NeedGauge;
}

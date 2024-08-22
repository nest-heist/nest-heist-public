using System;

/// <summary>
/// 몬스터 스킬 기본 정보
/// </summary>
public class MonsterSkillInfoData
{
    // 스킬 ID
    public string Id;

    // 스킬 이름 텍스트
    public string Name;

    // 스킬에 대해 한 줄로 설명해 주는 글
    public string Desc;

    // 스킬 범위
    public float Range;

    // 스킬 쿨타임
    public float CoolDown;

    // 스킬 시전시간
    public float CastTime;

    public float DamageMulti;

    public Define.AttributeType AttributeType;
}

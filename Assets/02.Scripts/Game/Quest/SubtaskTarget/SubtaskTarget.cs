
/// <summary>
/// 서브태스크 카운팅하는 타겟
/// 게임오브젝트 사용 X, 문자열만 쓰고 있다
/// [슬라임] 5마리 잡기
/// </summary>
public abstract class SubtaskTarget
{
    public abstract object Value { get; }

    public abstract bool IsEqual(object target);
}

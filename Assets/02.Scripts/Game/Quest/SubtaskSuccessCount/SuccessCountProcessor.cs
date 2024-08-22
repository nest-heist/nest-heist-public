/// <summary>
/// 성공 횟수 세는 방식
/// 슬라임 5마리 [없애기]
/// </summary>
public abstract class SuccessCountProcessor
{
    public abstract int Run(Subtask task, int currentSuccess, int successCount);
}

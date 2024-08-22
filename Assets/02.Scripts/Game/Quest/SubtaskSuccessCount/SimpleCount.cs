
/// <summary>
/// 일반적인 카운팅 방식 - 가장 많이 사용
/// </summary>
public class SimpleCount : SuccessCountProcessor
{
    public override int Run(Subtask task, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }
}


/// <summary>
/// successCount가 0보다 크면 성공한 것으로 카운팅하기
/// </summary>
public class PositiveCount : SuccessCountProcessor
{
    public override int Run(Subtask task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;
    }
}

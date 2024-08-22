
/// successCount가 0보다 작으면 성공한 것으로 카운팅하기
/// 5, -3 => 5 - - 3 = 8
/// </summary>
public class NegativeCount : SuccessCountProcessor
{
    public override int Run(Subtask task, int currentSuccess, int successCount)
    {
        return successCount < 0 ? currentSuccess - successCount : currentSuccess;
    }
}

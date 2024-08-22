
/// <summary>
/// 계속해서 성공하고 있는 횟수
/// </summary>
public class ContinousCount : SuccessCountProcessor
{
    public override int Run(Subtask task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }
}

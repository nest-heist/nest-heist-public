
/// <summary>
/// 값 업데이트만 받기 - 두번째로 자주 사용
/// </summary>
public class SimpleSet : SuccessCountProcessor
{
    public override int Run(Subtask task, int currentSuccess, int successCount)
    {
        return successCount;
    }
}
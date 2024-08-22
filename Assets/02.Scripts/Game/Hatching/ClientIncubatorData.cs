
/// <summary>
/// 클라에서 들고있어야 하는 데이터
/// </summary>
public class ClientIncubatorData
{
    // 알 서버에 전해주기 전까지 임시로 쓰는 데이터
    public ServerUserEggData TempEgg;
        
    public ClientIncubatorData(ServerIncubatorData serverIncubator)
    {
        this.TempEgg = null;
    }

    public void Reset()
    {
        this.TempEgg = null;
    }
}


/// <summary>
/// 서버 + 클라 데이터 저장 부화기 클래스
/// </summary>
public class Incubator
{ 
    public ServerIncubatorData ServerData;

    public ClientIncubatorData ClientIData;

    public Incubator(ServerIncubatorData serverIncubator)
    {
        ServerData = serverIncubator;
        ClientIData = new ClientIncubatorData(serverIncubator);
    }

    public void Reset()
    {
        ServerData.Reset();
        ClientIData.Reset();
    }
}

public class ServerIncubatorData
{
    public string Id;
    public string UserId;
    public string UserEggId;
    public string EggId; // 부화기에 들어간 알 타입 이미지 
    public string StartDate; // 년 월 일 시 초(부화시작)
    public string EndDate; // 년 월 일 시 초(부화끝)
    public int RemainingTime; // 남은 시간

    public ServerIncubatorData()
    {
        this.Id = "noId";
        this.UserId = "noId";
        this.UserEggId = Define.NoEgg;
        this.EggId = "noEggId";
        this.StartDate = "noStartDate";
        this.EndDate = "noEndDate";
        this.RemainingTime = -1;
    }

    public void Reset()
    {
        this.UserEggId = Define.NoEgg;
        this.EggId = "noEggId";
        this.StartDate = "noStartDate";
        this.EndDate = "noEndDate";
        this.RemainingTime = -1;
    }
}

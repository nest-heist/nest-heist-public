using System;

/// <summary>
/// 지금은 json파일로 저장, 나중에 서버에서 바로 받아오는 방식으로 변경
/// </summary>
[Serializable]
public class ServerUserData
{
    public string Id;
    public string Email;
    public string PasswordHash;
    public string LastLogin;
    public string CreatedAt;

    /// <summary>
    /// 처음 게임 시작할 때 초기화 해주는 값
    /// </summary>
    public ServerUserData()
    {
        Id = "PID00001";
        Email = "noEmail";
        PasswordHash = "noPassword";
        CreatedAt = "noCreatedAt";
        LastLogin = "noLastLogin";
    }
}

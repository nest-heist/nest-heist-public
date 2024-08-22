using UnityEngine;

using Newtonsoft.Json;

[System.Serializable]
public class ErrorLog
{
    public string timestamp;
    public string errorMessage;
    public string stackTrace;
    public string sceneName;
    public DeviceInfo deviceInfo;
    public string extraData;  // JSON 문자열로 저장

    public ErrorLog(string errorMessage, string stackTrace, string sceneName, object extraData = null)
    {
        this.timestamp = System.DateTime.UtcNow.ToString("o"); // ISO 8601 형식
        this.errorMessage = errorMessage;
        this.stackTrace = stackTrace;
        this.sceneName = sceneName;
        this.deviceInfo = new DeviceInfo();
        this.extraData = ConvertExtraDataToJson(extraData);
    }

    private string ConvertExtraDataToJson(object extraData)
    {
        if (extraData == null)
        {
            return null;
        }

        try
        {
            // extraData를 JSON 문자열로 변환
            return JsonConvert.SerializeObject(extraData);
        }
        catch (System.Exception ex)
        {
            // JSON 변환 실패 시, 에러 메시지 로깅
            Logging.LogError($"Failed to serialize extra data: {ex.Message}");
            return null;
        }
    }
}

[System.Serializable]
public class DeviceInfo
{
    public string platform;
    public string osVersion;
    public string deviceModel;

    public DeviceInfo()
    {
        platform = Application.platform.ToString();
        osVersion = SystemInfo.operatingSystem;
        deviceModel = SystemInfo.deviceModel;
    }
}

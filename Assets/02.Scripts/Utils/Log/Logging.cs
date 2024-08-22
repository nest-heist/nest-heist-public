using UnityEngine;

public static class Logging
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message)
    {
        Debug.Log(TimeStamp() + message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, Object context)
    {
        Debug.Log(TimeStamp() + message, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(TimeStamp() + message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(TimeStamp() + message, context);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object message)
    {
        Debug.LogError(TimeStamp() + message);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object message, Object context)
    {
        Debug.LogError(TimeStamp() + message, context);
    }

    public static string TimeStamp()
    {
        return System.DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.fff");
    }
}
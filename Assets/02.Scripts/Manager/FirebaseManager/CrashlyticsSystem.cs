using Firebase.Crashlytics;

public class CrashlyticsSystem
{
    public void Init()
    {
        Crashlytics.ReportUncaughtExceptionsAsFatal = true;
    }
}
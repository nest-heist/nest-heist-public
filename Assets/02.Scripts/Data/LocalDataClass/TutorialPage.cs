[System.Serializable]
public class TutorialPage
{
    public int Index;
    public string screen;

    // 생성자에서 기본값을 설정할 수 있음
    public TutorialPage(int idx, string screen)
    {
        Index = idx;
        this.screen = screen;
    }
}

[System.Serializable]
public class Tutorial
{
    public string Id;
    public bool IsEnabled;
    public TutorialPage[] Pages;
    public bool IsShown;

    // 기본값 설정을 위한 생성자
    public Tutorial(string tutorialId, bool isEnabled, TutorialPage[] tutorialPages)
    {
        Id = tutorialId;
        IsEnabled = isEnabled;
        Pages = tutorialPages;
        IsShown = false; 
    }
}

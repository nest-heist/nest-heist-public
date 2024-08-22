
[System.Serializable]
public class LocalPageTutorialData
{
    public Tutorial[] Tutorials;

    // 기본값 설정을 위한 생성자
    public LocalPageTutorialData()
    {
        Tutorials = new Tutorial[]
        {
            new Tutorial(
                Define.TutorialEggIncubator,
                true,
                new TutorialPage[]
                {
                    new TutorialPage(0, Define.TutorialScreen1),
                    new TutorialPage(1, Define.TutorialScreen2)
                }
            ),
            new Tutorial(
                Define.TutorialDungeon,
                true,
                new TutorialPage[0] // 빈 배열로 설정
            ),
            new Tutorial(
                Define.TutorialGeneral,
                true,
                new TutorialPage[]
                {
                    new TutorialPage(0, Define.TutorialScreen1),
                    new TutorialPage(1, Define.TutorialScreen2),
                    new TutorialPage(2, Define.TutorialScreen3)
                }
            )
        };
    }
}
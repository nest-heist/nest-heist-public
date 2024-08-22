using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PageTutorialSystem
{
    public LocalPageTutorialData TutorialData => DataManager.Instance.LocalDataSystem.PageTutorial;

    public bool IsTutorialEnabled(string tutorialId)
    {
        foreach (var tutorial in TutorialData.Tutorials)
        {
            if (tutorial.Id == tutorialId)
            {
                return tutorial.IsEnabled;
            }
        }
        return false;
    }

    public Tutorial GetTutorialById(string tutorialId)
    {
        foreach (var tutorial in TutorialData.Tutorials)
        {
            if (tutorial.Id == tutorialId)
            {
                return tutorial;
            }
        }
        return null;
    }

    public void ShowGeneralTutorialIfNotShown(Tutorial tutorial)
    {
        if (!tutorial.IsShown && tutorial.IsEnabled)
        {
            UIPopup_Tutorial popup = UIManager.Instance.InstanciateUIPopup<UIPopup_Tutorial>();

            foreach (var page in tutorial.Pages)
            {
                switch (page.screen)
                {
                    case Define.TutorialScreen1:
                        popup.Scrren1.SetActive(page.Index == popup.CurrentPage);
                        break;

                    case Define.TutorialScreen2:
                        popup.Scrren2.SetActive(page.Index == popup.CurrentPage);
                        break;

                    case Define.TutorialScreen3:
                        popup.Scrren3.SetActive(page.Index == popup.CurrentPage);
                        break;
                }
            }

            // 튜토리얼이 실행되었음을 기록
            tutorial.IsShown = true;

            // 튜토리얼 완료 후 데이터 저장
            DataManager.Instance.LocalDataSystem.SaveJsonData(TutorialData);
        }
    }

    public void ShowDungeonTutorialIfNotShown(Tutorial tutorial)
    {
        if (!tutorial.IsShown && tutorial.IsEnabled)
        {
            UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonTutorial>();

            tutorial.IsShown = true;
            DataManager.Instance.LocalDataSystem.SaveJsonData(TutorialData);
        }
    }

    public void ShowEggIncubatorTutorialIfNotShown(Tutorial tutorial)
    {
        if (!tutorial.IsShown && tutorial.IsEnabled)
        {
            UIPopup_EggIncubatorTutorial popup = UIManager.Instance.InstanciateUIPopup<UIPopup_EggIncubatorTutorial>();

            foreach (var page in tutorial.Pages)
            {
                if (page.screen == Define.TutorialScreen1)
                {
                    popup.Scrren1.SetActive(page.Index == popup.CurrentPage);
                }
                if (page.screen == Define.TutorialScreen2)
                {
                    popup.Scrren2.SetActive(page.Index == popup.CurrentPage);
                }
            }

            tutorial.IsShown = true;
            DataManager.Instance.LocalDataSystem.SaveJsonData(TutorialData);
        }
    }

    public void ReSetTutorial(Tutorial tutorial)
    {
        tutorial.IsEnabled = true;
        DataManager.Instance.LocalDataSystem.SaveJsonData(TutorialData);
    }

    public void ResetFirstConnect(Tutorial tutorial)
    {
        tutorial.IsShown = false;
        DataManager.Instance.LocalDataSystem.SaveJsonData(TutorialData);
    }
}
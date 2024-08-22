using UnityEngine.PlayerLoop;

public class ClickTutorialSystem
{
    public void Init()
    {
        
    }

    public void SaveTutorialIndex(int index)
    {
        DataManager.Instance.LocalDataSystem.ClickTutorial.CurrentTutorialIndex = index;
        DataManager.Instance.LocalDataSystem.SaveJsonData(DataManager.Instance.LocalDataSystem.ClickTutorial);
    }
}


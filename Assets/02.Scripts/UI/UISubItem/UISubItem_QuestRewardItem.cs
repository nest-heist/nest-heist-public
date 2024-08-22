using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISubItem_QuestRewardItem : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI ItemCountText;

    [Header("Image")]
    public Image ItemImage;

    public UISubItem_QuestRewardItem_Presenter Presenter;

    protected override void Awake()
    {
        base.Awake();

        Presenter = new UISubItem_QuestRewardItem_Presenter(this);
    }

    public void SetText(string itemCountText)
    {
        ItemCountText.text = itemCountText;
    }

    public void SetSprite(Sprite itemImage)
    {
        ItemImage.sprite = itemImage;
    }
}

public class UISubItem_QuestRewardItem_Presenter : Presenter
{
    private UISubItem_QuestRewardItem _view;
    private Reward _reward;

    public UISubItem_QuestRewardItem_Presenter(UISubItem_QuestRewardItem view)
    {
        _view = view;
    }

    public void SetReward(Reward reward)
    {
        _reward = reward;

        _view.SetText(reward.RewardInfo.Quantity.ToString());
        _view.SetSprite(_reward.Iconsprite);
    }
}

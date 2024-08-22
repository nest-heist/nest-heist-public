using TMPro;
using UnityEngine;

public class UISubItem_CouponWinner : UISubItem
{
    [Header("Text")]
    public TextMeshProUGUI EmailText;

    protected override void Start()
    {
        base.Start();
    }

    public void SetEmail(string email)
    {
        EmailText.text = email;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
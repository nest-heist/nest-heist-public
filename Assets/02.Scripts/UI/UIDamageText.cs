using UnityEngine;
using DG.Tweening;

public class UIDamageText : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOLocalMoveY(0.5f, 1.5f);
    }
}

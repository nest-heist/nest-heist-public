using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // 줄 데미지 양

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Health.Modify(-damageAmount);
        }
    }
}

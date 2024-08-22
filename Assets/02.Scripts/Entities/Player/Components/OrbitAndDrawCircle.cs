using UnityEngine;

public class OrbitAroundPlayer : MonoBehaviour
{
    // 플레이어 오브젝트를 참조 (원의 중심)
    public Transform CompossTransform;

    // 바라볼 목표 위치
    private Transform _targetTransform;

    // 원의 반지름
    private float orbitRadius = 0.7f;

    void Update()
    {
        if (_targetTransform != null && CompossTransform != null)
        {
            // Calculate direction from the character to the target
            Vector3 direction = _targetTransform.position - gameObject.transform.position;
            direction.z = 0; // Ensure it's a 2D direction

            // Calculate the position of the arrow based on the direction and radius
            Vector3 arrowPosition = gameObject.transform.position + direction.normalized * orbitRadius;

            // Update the position of the arrow
            CompossTransform.position = arrowPosition;

            // Rotate the arrow to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            CompossTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    // Initialize the target position (for example, from another script)
    public void Init(Transform newTarget)
    {
        CompossTransform.gameObject.SetActive(true);
        _targetTransform = newTarget;
    }
}

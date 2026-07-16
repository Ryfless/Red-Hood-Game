using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyDetection : MonoBehaviour
{
    private EnemyStats stats;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    private Transform playerTransform;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        // Try to find the PlayerController in the scene
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public bool IsPlayerVisible()
    {
        if (playerTransform == null) return false;

        // Check if player is in front of the enemy horizontally
        float directionX = playerTransform.position.x - transform.position.x;
        bool isPlayerInFront = (transform.right.x > 0 && directionX > 0) || (transform.right.x < 0 && directionX < 0);

        if (!isPlayerInFront)
        {
            return false;
        }

        // Check horizontal distance
        float horizontalDist = Mathf.Abs(directionX);
        if (horizontalDist > stats.VisionRange)
        {
            return false;
        }

        // Check height difference
        float heightDiff = Mathf.Abs(transform.position.y - playerTransform.position.y);
        if (heightDiff > stats.MaxHeightDifference)
        {
            return false;
        }

        // Raycast check: ground vs player (only Ground and Player)
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        LayerMask detectionMask = groundLayer | playerLayer;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            stats.VisionRange,
            detectionMask
        );

        if (hit.collider != null)
        {
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetPlayerPosition()
    {
        if (playerTransform != null)
        {
            return playerTransform.position;
        }
        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) stats = GetComponent<EnemyStats>();
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Vector3 endPoint = transform.position + transform.right * stats.VisionRange;
        Gizmos.DrawLine(transform.position, endPoint);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up * stats.MaxHeightDifference, endPoint + Vector3.up * stats.MaxHeightDifference);
        Gizmos.DrawLine(transform.position - Vector3.up * stats.MaxHeightDifference, endPoint - Vector3.up * stats.MaxHeightDifference);
    }
}

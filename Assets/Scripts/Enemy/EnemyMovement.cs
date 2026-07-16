using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStats stats;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
    }

    public void Move(float speed, bool headingRight)
    {
        float xVel = headingRight ? speed : -speed;
        rb.velocity = new Vector2(xVel, rb.velocity.y);
    }

    public void Stop()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void ApplyKnockback(Vector2 direction)
    {
        rb.velocity = Vector2.zero;
        Vector2 force = direction * stats.KnockbackForce;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CheckGround()
    {
        if (groundCheckPoint == null) return true;

        RaycastHit2D hit = Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            stats.GroundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    public bool CheckWall()
    {
        if (wallCheckPoint == null) return false;

        Vector2 dir = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position,
            dir,
            stats.WallCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) stats = GetComponent<EnemyStats>();
        if (stats == null) return;

        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * stats.GroundCheckDistance);
        }

        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            // Draw wall check line (defaulting to current right direction if transform is rotated)
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + transform.right * stats.WallCheckDistance);
        }
    }
}

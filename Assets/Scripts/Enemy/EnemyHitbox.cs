using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyHitbox : MonoBehaviour
{
    private EnemyStats stats;

    private void Awake()
    {
        stats = GetComponentInParent<EnemyStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHurtbox playerHurtbox = collision.GetComponent<PlayerHurtbox>();
        if (playerHurtbox != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            // Ensure some upward kick to knockback
            knockbackDir.y = 0.5f;
            knockbackDir.Normalize();

            playerHurtbox.TakeDamage(stats.AttackDamage, knockbackDir);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHurtbox playerHurtbox = collision.GetComponent<PlayerHurtbox>();
        if (playerHurtbox != null)
        {
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
            knockbackDir.y = 0.5f;
            knockbackDir.Normalize();

            playerHurtbox.TakeDamage(stats.AttackDamage, knockbackDir);
        }
    }
}

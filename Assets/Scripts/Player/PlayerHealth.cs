using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerAnimation))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerAnimation playerAnim;
    private Rigidbody2D rb;

    private float invincibilityTimer;
    private float knockbackTimer;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isDead;

    public bool IsInvincible => invincibilityTimer > 0f;
    public bool IsKnockedBack => knockbackTimer > 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerAnim = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackForce)
    {
        if (isDead || IsInvincible) return;

        stats.CurrentHP -= damage;
        invincibilityTimer = stats.InvincibleDuration;

        if (stats.CurrentHP <= 0)
        {
            Die();
            Debug.Log("Player has died.");
        }
        else
        {
            playerAnim.TriggerHit();
            ApplyKnockback(knockbackForce);
        }
    }

    private void ApplyKnockback(Vector2 force)
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
            knockbackTimer = knockbackDuration;
        }
    }

    private void Die()
    {
        isDead = true;
        playerAnim.TriggerDeath();

        // Disable player controls if necessary, handled in player logic or animation
        var pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}

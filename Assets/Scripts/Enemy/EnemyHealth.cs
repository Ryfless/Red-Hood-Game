using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyStateMachine))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyStateMachine stateMachine;
    private EnemyMovement movement;

    private bool isDead;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        stateMachine = GetComponent<EnemyStateMachine>();
        movement = GetComponent<EnemyMovement>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead) return;

        stats.CurrentHP -= damage;

        if (stats.CurrentHP <= 0)
        {
            Die();
        }
        else
        {
            stateMachine.TransitionToState(EnemyState.Hit);
            movement.ApplyKnockback(knockbackDirection);
        }
    }

    private void Die()
    {
        isDead = true;
        stateMachine.TransitionToState(EnemyState.Dead);

        // Disable collider
        var cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

        // Disable rigidbody if kinematic is preferred
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}

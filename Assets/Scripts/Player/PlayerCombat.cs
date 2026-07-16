using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private Transform attackPoint;

    private PlayerStats stats;
    private readonly HashSet<EnemyHurtbox> hitEnemies = new HashSet<EnemyHurtbox>();
    private Vector3 attackPointDefaultLocalPosition;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
            attackPointDefaultLocalPosition = attackPoint.localPosition;
        }
    }

    // Called via Animation Event
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            hitEnemies.Clear();
            attackHitbox.enabled = true;
        }
    }

    // Called via Animation Event or End of attack animation
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    public void SetAttackFacing(bool facingRight)
    {
        if (attackPoint == null)
            return;

        attackPoint.localPosition = new Vector3(
            facingRight
                ? Mathf.Abs(attackPointDefaultLocalPosition.x)
                : -Mathf.Abs(attackPointDefaultLocalPosition.x),
            attackPointDefaultLocalPosition.y,
            attackPointDefaultLocalPosition.z
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackHitbox != null && attackHitbox.enabled)
        {
            EnemyHurtbox enemyHurtbox = collision.GetComponent<EnemyHurtbox>();
            if (enemyHurtbox != null && hitEnemies.Add(enemyHurtbox))
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                knockbackDir.Normalize();

                enemyHurtbox.TakeDamage(stats.AttackDamage, knockbackDir);
            }
        }
    }
}

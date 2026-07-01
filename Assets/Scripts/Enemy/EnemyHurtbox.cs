using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyHurtbox : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, knockbackDirection);
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float baseKnockbackForce = 5f;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealth>();
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (playerHealth != null)
        {
            Vector2 force = knockbackDirection * baseKnockbackForce;
            playerHealth.TakeDamage(damage, force);
        }
    }
}

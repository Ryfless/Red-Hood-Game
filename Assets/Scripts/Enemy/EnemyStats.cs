using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int attackDamage = 50;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float visionRange = 8f;
    [SerializeField] private float memoryTime = 3f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float maxHeightDifference = 3f;
    [SerializeField] private float hitStunDuration = 0.5f;

    public int MaxHP => maxHP;
    public int AttackDamage => attackDamage;
    public float WalkSpeed => walkSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float VisionRange => visionRange;
    public float MemoryTime => memoryTime;
    public float GroundCheckDistance => groundCheckDistance;
    public float WallCheckDistance => wallCheckDistance;
    public float KnockbackForce => knockbackForce;
    public float MaxHeightDifference => maxHeightDifference;
    public float HitStunDuration => hitStunDuration;

    public int CurrentHP { get; set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }
}

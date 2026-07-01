using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float invincibleDuration = 1f;

    public int MaxHP => maxHP;
    public int AttackDamage => attackDamage;
    public float InvincibleDuration => invincibleDuration;

    public int CurrentHP { get; set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }
}

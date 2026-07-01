using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float destroyDelay = 1.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }

    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Death");
        // Destroy Enemy gameobject after animation duration or fixed delay
        Destroy(gameObject, destroyDelay);
    }
}

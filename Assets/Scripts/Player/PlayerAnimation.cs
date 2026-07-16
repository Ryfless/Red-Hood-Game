using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Death");
    }
}

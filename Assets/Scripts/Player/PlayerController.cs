using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Slide")]
    [SerializeField] private float slideDistance = 6f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideCooldown = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    // [SerializeField] private float attackDamage = 10f;
    // [SerializeField] private float attackRange = 1f;

    [Header("Slide Collision")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    private int playerLayerIndex;
    private int enemyLayerIndex;

    private Vector2 moveInput;

    private bool isGrounded;
    private bool facingRight = true;

    private float coyoteCounter;
    private float jumpBufferCounter;

    // Slide variables
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float slideCooldownTimer = 0f;
    
    // Attack variables
    private bool isAttacking = false;
    private float attackCooldownTimer = 0f;
    
    // Store original collider values
    private Vector2 originalColliderOffset;
    private Vector2 originalColliderSize;
    
    // Slide collider values
    private Vector2 slideColliderOffset = new Vector2(-0.0006659329f, 0.001997903f);
    private Vector2 slideColliderSize = new Vector2(0.2838699f, 0.1808023f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCombat = GetComponent<PlayerCombat>();
        
        // Store original collider values
        originalColliderOffset = boxCollider.offset;
        originalColliderSize = boxCollider.size;

        playerLayerIndex = LayerIndexFromMask(playerLayer);
        enemyLayerIndex = LayerIndexFromMask(enemyLayer);
    }

    private static int LayerIndexFromMask(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }
        return -1;
    }

    private void Update()
    {
        CheckGround();

        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleFlip();
        UpdateSlideTimer();
        UpdateAttackCooldown();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (playerHealth != null && playerHealth.IsKnockedBack)
        {
            return;
        }

        if (isSliding)
        {
            Slide();
        }
        else
        {
            Move();
        }
    }

    // ==========================
    // INPUT SYSTEM
    // ==========================

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (context.canceled)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(
                    rb.velocity.x,
                    rb.velocity.y * jumpCutMultiplier
                );
            }
        }
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.started && !isSliding && slideCooldownTimer <= 0f)
        {
            StartSlide();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && !isAttacking && attackCooldownTimer <= 0f)
        {
            StartAttack();
        }
    }

    // ==========================
    // MOVEMENT
    // ==========================

    [Header("Wall Check")]
    [SerializeField] private float wallCheckDistance = 0.15f;

    private void Move()
    {
        float horizontalInput = moveInput.x;

        if (!isGrounded && Mathf.Abs(horizontalInput) > 0.01f)
        {
            Bounds bounds = boxCollider.bounds;
            float dir = Mathf.Sign(horizontalInput);

            Vector2 rayOrigin = new Vector2(
                dir > 0 ? bounds.max.x : bounds.min.x,
                bounds.center.y
            );

            RaycastHit2D wallHit = Physics2D.Raycast(
                rayOrigin,
                Vector2.right * dir,
                wallCheckDistance,
                groundLayer
            );

            if (wallHit.collider != null)
                horizontalInput = 0;
        }

        rb.velocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.velocity.y
        );
    }

    private void Jump()
    {
        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce
        );

        animator.SetTrigger("Jump");

        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        slideCooldownTimer = slideCooldown;

        // Change collider for slide
        boxCollider.offset = slideColliderOffset;
        boxCollider.size = slideColliderSize;

        // Ignore collision with enemies during slide
        Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, true);
        if (playerHealth != null) playerHealth.IsSliding = true;

        animator.SetTrigger("Slide");
    }

    private void Slide()
    {
        float slideDirection = facingRight ? 1f : -1f;
        float slideSpeed = slideDistance / slideDuration;

        // Move player forward
        rb.velocity = new Vector2(
            slideSpeed * slideDirection,
            rb.velocity.y
        );

        slideTimer += Time.fixedDeltaTime;

        if (slideTimer >= slideDuration)
        {
            isSliding = false;
            
            // Restore collider to original values
            boxCollider.offset = originalColliderOffset;
            boxCollider.size = originalColliderSize;

            // Restore collision with enemies
            Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, false);
            if (playerHealth != null) playerHealth.IsSliding = false;
        }
    }

    private void UpdateSlideTimer()
    {
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackCooldownTimer = attackCooldown;

        animator.SetTrigger("Attack");
    }

    public void EnableAttackHitbox()
    {
        playerCombat?.EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        playerCombat?.DisableAttackHitbox();
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void UpdateAttackCooldown()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                attackCooldownTimer = 0f;
                if (isAttacking)
                {
                    isAttacking = false;
                }
            }
        }
    }

    private void HandleFlip()
    {
        if (moveInput.x > 0 && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
            playerCombat?.SetAttackFacing(facingRight);
        }
        else if (moveInput.x < 0 && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
            playerCombat?.SetAttackFacing(facingRight);
        }
    }

    // ==========================
    // GROUND CHECK
    // ==========================

    private void CheckGround()
    {
        Bounds bounds = boxCollider.bounds;
        Vector2 originBottomLeft = new Vector2(bounds.min.x + 0.01f, bounds.min.y);
        Vector2 originBottomRight = new Vector2(bounds.max.x - 0.01f, bounds.min.y);
        Vector2 originBottomCenter = new Vector2(bounds.center.x, bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(
            originBottomLeft, Vector2.down, groundCheckDistance, groundLayer
        );
        RaycastHit2D hitRight = Physics2D.Raycast(
            originBottomRight, Vector2.down, groundCheckDistance, groundLayer
        );
        RaycastHit2D hitCenter = Physics2D.Raycast(
            originBottomCenter, Vector2.down, groundCheckDistance, groundLayer
        );

        isGrounded = hitLeft.collider != null || hitRight.collider != null || hitCenter.collider != null;
    }

    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpBuffer()
    {
        jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f &&
            coyoteCounter > 0f)
        {
            Jump();
        }
    }

    // ==========================
    // ANIMATOR
    // ==========================

    private void UpdateAnimator()
    {
        animator.SetFloat(
            "Speed",
            Mathf.Abs(moveInput.x)
        );

        animator.SetFloat(
            "YVelocity",
            rb.velocity.y
        );

        animator.SetBool(
            "IsGrounded",
            isGrounded
        );

        animator.SetBool(
            "IsSliding",
            isSliding
        );

        animator.SetBool(
            "IsAttacking",
            isAttacking
        );
    }


    // ==========================
    // DEBUG
    // ==========================

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        Vector2 originBottomLeft = new Vector2(bounds.min.x + 0.02f, bounds.min.y);
        Vector2 originBottomRight = new Vector2(bounds.max.x - 0.02f, bounds.min.y);
        Vector2 originBottomCenter = new Vector2(bounds.center.x, bounds.min.y);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(originBottomLeft, originBottomLeft + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(originBottomCenter, originBottomCenter + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(originBottomRight, originBottomRight + Vector2.down * groundCheckDistance);

        Vector2 leftOrigin = new Vector2(bounds.min.x, bounds.center.y);
        Vector2 rightOrigin = new Vector2(bounds.max.x, bounds.center.y);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftOrigin, leftOrigin + Vector2.left * wallCheckDistance);
        Gizmos.DrawLine(rightOrigin, rightOrigin + Vector2.right * wallCheckDistance);
    }
}
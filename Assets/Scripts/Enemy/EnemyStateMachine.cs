using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Hit,
    Dead
}

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyAnimation))]
public class EnemyStateMachine : MonoBehaviour
{
    private EnemyState currentState;
    private EnemyStats stats;
    private EnemyMovement movement;
    private EnemyDetection detection;
    private EnemyHealth health;
    private EnemyAnimation enemyAnim;

    [Header("Idle Settings")]
    [SerializeField] private float idleDurationMin = 1f;
    [SerializeField] private float idleDurationMax = 3f;

    [Header("Patrol Settings")]
    [SerializeField] private float walkDurationMin = 2f;
    [SerializeField] private float walkDurationMax = 5f;

    private float stateTimer;
    private bool isFacingRight = true;

    // Memory system variables
    private float memoryTimer;
    private Vector2 lastKnownPlayerPosition;
    private bool hasLastKnownPosition;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        movement = GetComponent<EnemyMovement>();
        detection = GetComponent<EnemyDetection>();
        health = GetComponent<EnemyHealth>();
        enemyAnim = GetComponent<EnemyAnimation>();
    }

    private void Start()
    {
        TransitionToState(EnemyState.Idle);
    }

    private void Update()
    {
        // Handle State logic
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Hit:
                UpdateHit();
                break;
            case EnemyState.Dead:
                break;
        }
    }

    private void FixedUpdate()
    {
        bool isMovingNow = false;

        if (currentState == EnemyState.Patrol)
        {
            movement.Move(stats.WalkSpeed, isFacingRight);
            isMovingNow = true;
        }
        else if (currentState == EnemyState.Chase)
        {
            if (detection.IsPlayerVisible())
            {
                Vector2 playerPos = detection.GetPlayerPosition();
                bool targetRight = playerPos.x > transform.position.x;
                
                if (targetRight != isFacingRight)
                {
                    Flip();
                }

                if (movement.CheckGround() && !movement.CheckWall())
                {
                    movement.Move(stats.ChaseSpeed, isFacingRight);
                    isMovingNow = true;
                }
                else
                {
                    movement.Stop();
                    isMovingNow = false;
                }
            }
            else if (hasLastKnownPosition)
            {
                bool targetRight = lastKnownPlayerPosition.x > transform.position.x;
                
                if (targetRight != isFacingRight)
                {
                    Flip();
                }

                if (movement.CheckGround() && !movement.CheckWall())
                {
                    movement.Move(stats.ChaseSpeed, isFacingRight);
                    isMovingNow = true;
                }
                else
                {
                    movement.Stop();
                    isMovingNow = false;
                    hasLastKnownPosition = false;
                }
            }
            else
            {
                movement.Stop();
                isMovingNow = false;
            }
        }
        else
        {
            movement.Stop();
            isMovingNow = false;
        }

        enemyAnim.SetMoving(isMovingNow);
    }

    public void TransitionToState(EnemyState newState)
    {
        if (currentState == EnemyState.Dead) return; // Cannot escape death

        // Exit current state logic
        switch (currentState)
        {
            case EnemyState.Chase:
                break;
        }

        currentState = newState;

        // Enter new state logic
        switch (currentState)
        {
            case EnemyState.Idle:
                stateTimer = Random.Range(idleDurationMin, idleDurationMax);
                break;

            case EnemyState.Patrol:
                stateTimer = Random.Range(walkDurationMin, walkDurationMax);
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Hit:
                stateTimer = stats.HitStunDuration;
                enemyAnim.TriggerHit();
                movement.Stop();
                break;

            case EnemyState.Dead:
                movement.Stop();
                enemyAnim.TriggerDeath();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (detection.IsPlayerVisible())
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            // Walk speed/direction determination
            if (Random.value > 0.5f)
            {
                Flip();
            }
            TransitionToState(EnemyState.Patrol);
        }
    }

    private void UpdatePatrol()
    {
        if (detection.IsPlayerVisible())
        {
            TransitionToState(EnemyState.Chase);
            return;
        }

        // Check walls and platform edges
        if (movement.CheckWall() || !movement.CheckGround())
        {
            Flip();
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            TransitionToState(EnemyState.Idle);
        }
    }

    private void UpdateChase()
    {
        if (detection.IsPlayerVisible())
        {
            lastKnownPlayerPosition = detection.GetPlayerPosition();
            hasLastKnownPosition = true;
            memoryTimer = stats.MemoryTime;
        }
        else
        {
            if (hasLastKnownPosition)
            {
                // Head to last known position
                float distToLastKnown = Mathf.Abs(transform.position.x - lastKnownPlayerPosition.x);
                if (distToLastKnown < 0.2f)
                {
                    hasLastKnownPosition = false;
                }

                // Check walls and edges even while chasing to avoid falling
                if (movement.CheckWall() || !movement.CheckGround())
                {
                    // If hit obstacle, memory position is unreachable horizontally
                    hasLastKnownPosition = false;
                }
            }

            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f && !hasLastKnownPosition)
            {
                TransitionToState(EnemyState.Idle);
            }
        }
    }

    private void UpdateHit()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            // Return to previous active state or default to Idle
            if (detection.IsPlayerVisible())
            {
                TransitionToState(EnemyState.Chase);
            }
            else
            {
                TransitionToState(EnemyState.Idle);
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}

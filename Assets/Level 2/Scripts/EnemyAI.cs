using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI with Finite State Machine: Idle, Patrol, Chase, Attack, Die.
/// Uses NavMeshAgent for pathfinding.
/// Attach to Enemy GameObject (requires NavMeshAgent).
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack, Die }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float fieldOfViewAngle = 120f;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamage = 10f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;

    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Idle;
    private Transform player;
    private int currentPatrolIndex;
    private float patrolWaitTimer;
    private float attackTimer;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (patrolPoints.Length > 0)
        {
            TransitionTo(EnemyState.Patrol);
        }
        else
        {
            TransitionTo(EnemyState.Idle);
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Die) return;

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
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
    }

    // --- STATE UPDATES ---

    private void UpdateIdle()
    {
        if (CanDetectPlayer())
        {
            TransitionTo(EnemyState.Chase);
        }
    }

    private void UpdatePatrol()
    {
        if (CanDetectPlayer())
        {
            TransitionTo(EnemyState.Chase);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                MoveToNextPatrolPoint();
            }
        }
    }

    private void UpdateChase()
    {
        if (player == null)
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        float distToPlayer = Vector3.Distance(
            transform.position, player.position
        );

        if (distToPlayer <= attackRange)
        {
            TransitionTo(EnemyState.Attack);
            return;
        }

        if (distToPlayer > detectionRange * 1.5f)
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        agent.SetDestination(player.position);
    }

    private void UpdateAttack()
    {
        if (player == null)
        {
            TransitionTo(EnemyState.Patrol);
            return;
        }

        float distToPlayer = Vector3.Distance(
            transform.position, player.position
        );

        if (distToPlayer > attackRange * 1.3f)
        {
            TransitionTo(EnemyState.Chase);
            return;
        }

        // Face the player
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);

        agent.SetDestination(transform.position);

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
        }
    }

    // --- STATE TRANSITIONS ---

    private void TransitionTo(EnemyState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                break;

            case EnemyState.Patrol:
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                MoveToNextPatrolPoint();
                break;

            case EnemyState.Chase:
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                break;

            case EnemyState.Attack:
                agent.isStopped = true;
                attackTimer = 0f;
                break;

            case EnemyState.Die:
                agent.isStopped = true;
                agent.enabled = false;
                break;
        }
    }

    // --- HELPERS ---

    private bool CanDetectPlayer()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRange) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > fieldOfViewAngle * 0.5f) return false;

        // Line of sight check
        if (Physics.Raycast(
                transform.position + Vector3.up,
                dirToPlayer,
                out RaycastHit hit,
                detectionRange))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        patrolWaitTimer = 0f;
    }

    private void PerformAttack()
    {
        IDamageable target = player.GetComponent<IDamageable>();
        target?.TakeDamage(attackDamage,"EnemyAI","Player");
        Debug.Log($"[EnemyAI] Attacked player for {attackDamage} damage.");
    }

    /// <summary>
    /// Called by EnemyHealth when HP reaches zero.
    /// </summary>
    public void Die()
    {
        TransitionTo(EnemyState.Die);
    }

    /// <summary>
    /// Sets patrol points at runtime (used by WaveManager
    /// when spawning enemies dynamically).
    /// </summary>
    public void SetPatrolPoints(Transform[] points)
    {
        patrolPoints = points;

        if (patrolPoints.Length > 0 && currentState == EnemyState.Idle)
        {
            TransitionTo(EnemyState.Patrol);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

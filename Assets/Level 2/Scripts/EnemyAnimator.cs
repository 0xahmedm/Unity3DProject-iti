using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Bridges EnemyAI state to the Animator.
/// Reads current state and drives animation parameters.
/// Attach to the Enemy GameObject (same as EnemyAI).
/// </summary>
[RequireComponent(typeof(EnemyAI))]
public class EnemyAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Smoothing")]
    [SerializeField] private float speedDampTime = 0.1f;

    // Cached hashes
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private NavMeshAgent agent;
    private EnemyAI enemyAI;
    private EnemyHealth enemyHealth;
    private bool hasDied;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDied += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDied -= HandleDeath;
        }
    }

    private void Update()
    {
        if (hasDied) return;

        UpdateSpeed();
        UpdateAttacking();
    }

    private void UpdateSpeed()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat(SpeedHash, speed, speedDampTime, Time.deltaTime);
    }

    private void UpdateAttacking()
    {
        bool attacking = enemyAI.CurrentState == EnemyAI.EnemyState.Attack;
        animator.SetBool(IsAttackingHash, attacking);
    }

    private void HandleDeath()
    {
        hasDied = true;
        animator.SetTrigger(DieHash);
    }
}

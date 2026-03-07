using UnityEngine;

public class L1EnemyChase : MonoBehaviour
{
    public Transform player;

    [Header("Speed")]
    public float startSpeed = 14f;
    public float maxSpeed = 19f;
    public float acceleration = 0.5f;

    [Header("Catch")]
    public float catchDistance = 3.5f;

    [Header("Lane Follow")]
    public float laneSwitchSpeed = 12f;
    public float leftLaneZ = -24f;
    public float middleLaneZ = -39.5f;
    public float rightLaneZ = -55f;

    [Header("Spawn Offset")]
    public float spawnOffsetX = -20f;

    [Header("Sounds")]
    public AudioSource runLoop;
    public AudioSource tauntSound;

    public bool stopChasing;

    private float currentSpeed;
    private float targetZ;
    private Animator animator;
    private L1PlayerHealth playerHealth;
    private bool caught;

    void Start()
    {
        currentSpeed = startSpeed;
        animator = GetComponentInChildren<Animator>();
        playerHealth = player.GetComponent<L1PlayerHealth>();
        targetZ = middleLaneZ;

        // Position enemy behind player or checkpoint
        Vector3 pos = transform.position;

        if (PlayerPrefs.HasKey("CheckpointX"))
            pos.x = PlayerPrefs.GetFloat("CheckpointX") + spawnOffsetX;
        else
            pos.x = player.position.x + spawnOffsetX;

        transform.position = pos;

        if (runLoop != null)
            runLoop.Play();
    }

    void Update()
    {
        if (stopChasing) return;

        if (playerHealth != null && playerHealth.isDead)
        {
            StopAndTaunt();
            return;
        }

        MoveTowardPlayer();
        UpdateAnimator();
    }

    void MoveTowardPlayer()
    {
        float distance = player.position.x - transform.position.x;

        if (distance > catchDistance)
        {
            caught = false;

            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, startSpeed, maxSpeed);

            Vector3 move = Vector3.right * currentSpeed * Time.deltaTime;
            transform.position += move;

            float playerZ = player.position.z;

            if (playerZ > -32f)
                targetZ = leftLaneZ;
            else if (playerZ < -47f)
                targetZ = rightLaneZ;
            else
                targetZ = middleLaneZ;

            Vector3 pos = transform.position;
            pos.z = Mathf.Lerp(pos.z, targetZ, laneSwitchSpeed * Time.deltaTime);
            transform.position = pos;
        }
        else
        {
            StopAndTaunt();
        }
    }

    void StopAndTaunt()
    {
        if (!caught)
        {
            caught = true;
            currentSpeed = 0;

            animator.SetBool("IsRunning", false);
            animator.SetTrigger("Taunt");

            if (runLoop != null)
                runLoop.Stop();

            if (tauntSound != null)
                tauntSound.Play();

            if (playerHealth != null && !playerHealth.isDead)
                playerHealth.TakeDamage(playerHealth.currentHealth);
        }
    }

    void UpdateAnimator()
    {
        if (caught) return;
        animator.SetBool("IsRunning", true);
    }
}
using System.Collections;
using UnityEngine;

public class L1PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject fadeout;

    [Header("Spawn Protection")]
    public float spawnProtectionDuration = 3f;
    private bool isInvincible;

    private Animator animator;
    private L1PlayerMovement movement;
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        movement = GetComponent<L1PlayerMovement>();

        // Give invincibility on spawn if checkpoint is being used
        if (movement.useCheckpoint && PlayerPrefs.HasKey("CheckpointX"))
            StartCoroutine(SpawnProtection());
    }

    IEnumerator SpawnProtection()
    {
        isInvincible = true;
        Debug.Log("Spawn protection active");
        yield return new WaitForSeconds(spawnProtectionDuration);
        isInvincible = false;
        Debug.Log("Spawn protection ended");
    }

    public void TakeDamage(int amount)
    {
        // Ignore damage during spawn protection
        if (isDead || isInvincible) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }
        else
        {
            Stumble();
        }
    }

    void Stumble()
    {
        animator.SetTrigger("Stumble");
        movement.enabled = false;
        Invoke(nameof(Recover), 0.2f);
    }

    void Recover()
    {
        if (!isDead)
            movement.enabled = true;
    }

    IEnumerator Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
        movement.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(3f);
        fadeout.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        L1GameManager.instance.GameOver();
    }
}
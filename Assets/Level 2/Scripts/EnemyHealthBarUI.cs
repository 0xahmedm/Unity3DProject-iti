using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Floating health bar that appears above enemies.
/// Faces the camera (billboard) and updates from EnemyHealth events.
/// Attach to a World Space Canvas as a child of the Enemy GameObject.
/// </summary>
public class EnemyHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Settings")]
    [SerializeField] private bool hideWhenFull = true;

    private Canvas canvas;
    private Camera mainCam;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
        }
    }

    private void Start()
    {
        mainCam = Camera.main;

        if (mainCam == null)
        {
            var brain = FindAnyObjectByType<Unity.Cinemachine.CinemachineBrain>();
            if (brain != null) mainCam = brain.OutputCamera;
        }

        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }

        if (hideWhenFull && canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += UpdateBar;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= UpdateBar;
        }
    }

    private void LateUpdate()
    {
        if (mainCam == null) return;

        // Billboard — always face the camera
        transform.LookAt(
            transform.position + mainCam.transform.forward
        );
    }

    private void UpdateBar(float current, float max)
    {
        float ratio = current / max;

        if (healthSlider != null)
        {
            healthSlider.value = ratio;
        }

        // Show bar once damaged
        if (hideWhenFull && canvas != null)
        {
            canvas.enabled = ratio < 1f;
        }
    }
}

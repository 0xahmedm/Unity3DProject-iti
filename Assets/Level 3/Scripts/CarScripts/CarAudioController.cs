using UnityEngine;

public class CarAudioController : MonoBehaviour
{
    [Header("Engine Sounds")]
    [SerializeField] private AudioClip engineIdleClip;
    [SerializeField] private float minEnginePitch = 0.8f;
    [SerializeField] private float maxEnginePitch = 2.5f;
    [SerializeField] private float engineVolume = 0.8f;

    [Header("Car Sounds")]
    [SerializeField] private AudioClip brakeClip;
    [SerializeField] private AudioClip driftClip;
    [SerializeField] private AudioClip crashClip;

    private CarController carController;
    private const string ENGINE_SOUND_ID = "car_engine";
    private const string DRIFT_SOUND_ID = "car_drift";

    private void Start()
    {
        carController = GetComponent<CarController>();

    
        if (engineIdleClip != null)
        {
            AudioManager.Instance?.PlayLoopingSound(ENGINE_SOUND_ID, engineIdleClip, engineVolume, minEnginePitch);
        }
    }

    private void Update()
    {
        UpdateEngineSound();
        HandleDrifting();
    }

    private void UpdateEngineSound()
    {
        if (AudioManager.Instance == null || carController == null) return;

        float speed = carController.GetCurrentSpeed();
        float speedFactor = Mathf.Clamp01(speed / 100f);


        float targetPitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedFactor);
    
        float targetVolume = Mathf.Lerp(0.5f, 1f, speedFactor) * engineVolume;

        AudioManager.Instance.UpdateLoopingSound(ENGINE_SOUND_ID, targetVolume, targetPitch);
    }

    private void HandleDrifting()
    {
        float speed = carController.GetCurrentSpeed();
        bool isDrifting = Input.GetKey(KeyCode.Space) && speed > 30f;

        if (isDrifting && !AudioManager.Instance.IsLoopingSoundPlaying(DRIFT_SOUND_ID))
        {
            AudioManager.Instance?.PlayLoopingSound(DRIFT_SOUND_ID, driftClip, 0.7f);
        }
        else if (!isDrifting && AudioManager.Instance.IsLoopingSoundPlaying(DRIFT_SOUND_ID))
        {
            AudioManager.Instance?.StopLoopingSound(DRIFT_SOUND_ID);
        }
    }

    public void PlayBrakeSound()
    {
        AudioManager.Instance?.PlaySound(brakeClip, 0.8f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5f)
        {
            AudioManager.Instance?.PlaySound(crashClip, 1f);
        }
    }

    private void OnDestroy()
    {
        AudioManager.Instance?.StopLoopingSound(ENGINE_SOUND_ID);
        AudioManager.Instance?.StopLoopingSound(DRIFT_SOUND_ID);
    }
}
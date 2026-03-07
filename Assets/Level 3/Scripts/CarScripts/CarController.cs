using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Axel { Front, Rear }

[System.Serializable]
public struct Wheel
{
    public GameObject wheelModel;
    public WheelCollider wheelCollider;
    public Axel axel;
}

public class CarController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Driving")]
    [SerializeField] private float maxAcceleration = 150f;
    [SerializeField] private float brakePower = 300f;
    [SerializeField] private float reverseMultiplier = 0.4f;
    [SerializeField] private float naturalDeceleration = 80f;
    
    [Header("Steering")]
    [SerializeField] private float steeringSpeed = 8f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private AnimationCurve steeringCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.3f);
    
    [Header("Wheels")]
    [SerializeField] private List<Wheel> wheels;
    [SerializeField] private Vector3 centerOfMass = new Vector3(0f, -0.5f, 0f);
    
    [Header("Visual Effects")]
    [SerializeField] private Transform carBody;
    [SerializeField] private float tiltAmount = 4f;
    [SerializeField] private float tiltSpeed = 4f;
    [SerializeField] private ParticleSystem[] tireSmoke;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speedDisplay;
    #endregion

    #region Private Variables
    private Rigidbody rb;
    private PlayerInput playerInput;

    private float currentSpeed;
    private float currentTilt;
    private float[] wheelRotations;

    // Wall hit detection
    private float timeSinceLastCollision = 999f;
    private const float COLLISION_RECOVERY_TIME = 0.15f;
    private bool isCollisionStunned = false; // Flag to prevent input during collision recovery

    // Idle timer to prevent snap-stop after collision
    private float idleTimer = 0f;
    private const float IDLE_STOP_DELAY = 0.5f;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        rb.centerOfMass = centerOfMass;
        rb.linearDamping = 0.3f;
        rb.angularDamping = 0.2f;

        wheelRotations = new float[wheels.Count];
    }

    private void Update()
    {
        AnimateWheels();
        TiltCarBody();
        timeSinceLastCollision += Time.deltaTime;
        
        // Clear stun flag after recovery time
        if (isCollisionStunned && timeSinceLastCollision >= COLLISION_RECOVERY_TIME)
        {
            isCollisionStunned = false;
        }

        if (speedDisplay != null)
        {
            speedDisplay.text = $"{GetCurrentSpeed():0} km/h";
        }
    }

    private void FixedUpdate()
    {
        UpdateSpeed();
        Move();
        Steer();
        Brake();
        KillCreep();
    }

    private void OnCollisionEnter(Collision collision)
    {
        timeSinceLastCollision = 0f;
        isCollisionStunned = true; // Set stun flag
        idleTimer = 0f;

        // Immediately kill motor torque and apply brakes
        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
            wheel.wheelCollider.brakeTorque = brakePower * 1200f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Keep stun active while still colliding
        if (timeSinceLastCollision < 0.1f)
        {
            isCollisionStunned = true;
        }
    }
    #endregion

    #region Movement
    private void UpdateSpeed()
    {
        currentSpeed = rb.linearVelocity.magnitude * 3.6f;
        // Higher damping when coasting, low when driving
        rb.linearDamping = HasValidInput() ? 0.2f : 1.2f;
    }

    private void Move()
    {
        // Don't apply movement if stunned from collision
        if (isCollisionStunned)
        {
            // Keep brakes applied during stun
            foreach (Wheel wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = brakePower * 800f;
            }
            return;
        }

        if (HasVerticalInput())
        {
            idleTimer = 0f;
            ApplyMotorTorque();
        }
        else
        {
            ApplyNaturalDeceleration();
        }
    }

    private void ApplyMotorTorque()
    {
        float acceleration = maxAcceleration;
        if (playerInput.VerticalInput < 0)
            acceleration *= reverseMultiplier;

        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = playerInput.VerticalInput * acceleration * 500f;
            wheel.wheelCollider.brakeTorque = 0f;
        }
    }

    private void ApplyNaturalDeceleration()
    {
        // Scale brake torque with speed so decel feels proportional
        float speedRatio = Mathf.Clamp(currentSpeed / 30f, 0.2f, 1f);
        float brakeTorque = naturalDeceleration * speedRatio * 800f;

        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
            wheel.wheelCollider.brakeTorque = brakeTorque;
        }

        // Only full-stop after being genuinely idle — not from a wall bounce
        bool justHitWall = timeSinceLastCollision < COLLISION_RECOVERY_TIME;
        if (currentSpeed < 1.5f && !justHitWall)
        {
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= IDLE_STOP_DELAY)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else if (currentSpeed >= 1.5f)
        {
            idleTimer = 0f;
        }
    }

    private void KillCreep()
    {
        // Don't apply creep reduction if stunned
        if (isCollisionStunned) return;
        
        // Gentle damping when nearly stopped and no input — avoids physics micro-sliding
        bool justHitWall = timeSinceLastCollision < COLLISION_RECOVERY_TIME;
        if (!HasVerticalInput() && !IsBraking() && currentSpeed < 2f && !justHitWall)
        {
            rb.linearVelocity *= 0.88f;
        }
    }

    private bool HasVerticalInput() => playerInput.VerticalInput != 0;
    private bool HasValidInput() => HasVerticalInput() && !isCollisionStunned; // Check if input should be processed
    #endregion

    #region Braking
    private void Brake()
    {
        // Don't allow manual braking during stun (brakes are already applied)
        if (isCollisionStunned) return;
        
        if (IsBraking())
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = brakePower * 1200f;
            }
            HandleTireSmoke(true);
        }
        else
        {
            HandleTireSmoke(false);
        }
    }

    public bool IsBraking() => Input.GetKey(KeyCode.Space) && !isCollisionStunned;

    private void HandleTireSmoke(bool active)
    {
        if (tireSmoke == null) return;
        foreach (var smoke in tireSmoke)
        {
            if (active && currentSpeed > 20f && !smoke.isPlaying)
                smoke.Play();
            else if (!active && smoke.isPlaying)
                smoke.Stop();
        }
    }
    #endregion

    #region Steering
    private void Steer()
    {
        // Disable steering during stun for better control
        if (isCollisionStunned) return;
        
        float speedFactor = Mathf.Clamp01(currentSpeed / 100f);
        float adjustedMax = maxSteerAngle * steeringCurve.Evaluate(speedFactor);

        foreach (Wheel wheel in wheels)
        {
            if (wheel.axel != Axel.Front) continue;

            float target = playerInput.HorizontalInput * adjustedMax;
            wheel.wheelCollider.steerAngle = Mathf.Lerp(
                wheel.wheelCollider.steerAngle,
                target,
                steeringSpeed * Time.fixedDeltaTime
            );
        }
    }
    #endregion

    #region Visual Effects
    private void AnimateWheels()
    {
        if (wheels.Count == 0) return;
        
        float wheelRadius = wheels[0].wheelCollider.radius;

        // Use LOCAL forward velocity only — so a sideways bounce after hitting a wall
        // doesn't register as forward motion and spin the wheels
        float localForwardVelocity = transform.InverseTransformDirection(rb.linearVelocity).z;

        // deg/s = (m/s / radius) * Rad2Deg
        float degreesPerSecond = (localForwardVelocity / wheelRadius) * Mathf.Rad2Deg;

        for (int i = 0; i < wheels.Count; i++)
        {
            Wheel wheel = wheels[i];
            wheelRotations[i] += degreesPerSecond * Time.deltaTime;

            wheel.wheelModel.transform.localRotation = Quaternion.Euler(
                0f,
                wheel.wheelCollider.steerAngle,
                wheelRotations[i]
            );
        }
    }

    private void TiltCarBody()
    {
        if (carBody == null) return;
        float targetTilt = -playerInput.HorizontalInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        carBody.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
    }
    #endregion

    #region Public API
    public float GetCurrentSpeed() => currentSpeed;
    public float GetCurrentSpeedNormalized() => Mathf.Clamp01(currentSpeed / 150f);
    public float GetSteeringInput() => playerInput.HorizontalInput;
    public float GetThrottleInput() => playerInput.VerticalInput;
    public bool IsStunned() => isCollisionStunned; // Useful for UI feedback
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(centerOfMass), 0.2f);
        
        // Visualize stun state
        if (isCollisionStunned)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
    #endregion
}
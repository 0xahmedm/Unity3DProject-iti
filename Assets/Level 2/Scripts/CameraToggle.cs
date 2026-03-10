using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Toggles between FPS and TPS Cinemachine cameras.
/// Press V to switch perspectives.
/// Attach to the Player GameObject.
/// </summary>
public class CameraToggle : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera fpsCam;
    [SerializeField] private CinemachineCamera tpsCam;

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.V;
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    private bool isThirdPerson;

    /// <summary>
    /// True when in third-person mode. Other scripts can
    /// read this to adjust behavior per camera mode.
    /// </summary>
    public bool IsThirdPerson => isThirdPerson;

    private void Start()
    {
        SetFPSMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isThirdPerson = !isThirdPerson;
            ApplyCameraPriorities();
        }
    }

    private void SetFPSMode()
    {
        isThirdPerson = false;
        ApplyCameraPriorities();
    }

    private void ApplyCameraPriorities()
    {
        fpsCam.Priority = isThirdPerson ? inactivePriority : activePriority;
        tpsCam.Priority = isThirdPerson ? activePriority : inactivePriority;
    }
}

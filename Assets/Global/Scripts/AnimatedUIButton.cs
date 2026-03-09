using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UITweenSfx : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Tween Target")]
    [SerializeField] private RectTransform target; // if null -> this RectTransform

    [Header("Tween Settings (Scale)")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float tweenDuration = 0.08f;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Optional Click Pulse")]
    [SerializeField] private bool clickPulse = true;
    [SerializeField] private float clickPulseScale = 1.08f;
    [SerializeField] private float clickPulseDuration = 0.10f;

    [Header("SFX")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioClip clickClip;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;

    [Header("Rules")]
    [SerializeField] private bool requireInteractable = true;

    private Vector3 baseScale;
    private Coroutine tweenRoutine;
    private Coroutine pulseRoutine;

    private bool hovered;
    private bool pressed;

    private Selectable selectable;

    private void Awake()
    {
        if (!target) target = transform as RectTransform;
        baseScale = target ? target.localScale : Vector3.one;

        selectable = GetComponent<Selectable>();
    }

    private void OnEnable()
    {
        hovered = false;
        pressed = false;
        TweenTo(baseScale, instant: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanInteract()) return;

        hovered = true;
        Play(enterClip);
        ApplyState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanInteract()) return;

        hovered = false;
        pressed = false;
        Play(exitClip);
        ApplyState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanInteract()) return;

        pressed = true;
        ApplyState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!CanInteract()) return;

        pressed = false;
        ApplyState();
    }

public void OnPointerClick(PointerEventData eventData)
{
    if (!CanInteract()) return;

    Play(clickClip);

    
    if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return;

    if (clickPulse)
    {
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(Pulse());
    }
}

    private void ApplyState()
    {
        if (!target) return;

        Vector3 desired =
            pressed ? baseScale * pressedScale :
            hovered ? baseScale * hoverScale :
            baseScale;

        TweenTo(desired);
    }

    private void TweenTo(Vector3 desired, bool instant = false)
    {
        if (!target) return;

        if (instant || tweenDuration <= 0f)
        {
            if (tweenRoutine != null) StopCoroutine(tweenRoutine);
            target.localScale = desired;
            return;
        }

        if (tweenRoutine != null) StopCoroutine(tweenRoutine);
        tweenRoutine = StartCoroutine(TweenScale(desired, tweenDuration));
    }

    private IEnumerator TweenScale(Vector3 desired, float duration)
    {
        Vector3 start = target.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);

            
            a = EaseOutCubic(a);

            target.localScale = Vector3.LerpUnclamped(start, desired, a);
            yield return null;
        }

        target.localScale = desired;
        tweenRoutine = null;
    }

    private IEnumerator Pulse()
    {
        if (gameObject.activeSelf == false) yield break;
        Vector3 stateScale =
            pressed ? baseScale * pressedScale :
            hovered ? baseScale * hoverScale :
            baseScale;

        Vector3 up = baseScale * clickPulseScale;

        // go up
        yield return TweenScale(up, clickPulseDuration * 0.5f);
        // go back to state
        yield return TweenScale(stateScale, clickPulseDuration * 0.5f);

        pulseRoutine = null;
    }

private bool CanInteract()
{
    if (!isActiveAndEnabled || !gameObject.activeInHierarchy) return false;
    if (!requireInteractable) return true;

    var selectable = GetComponent<Selectable>();
    return selectable == null || (selectable.IsActive() && selectable.IsInteractable());
}


    private void Play(AudioClip clip)
    {
        if (!source || !clip) return;
        source.PlayOneShot(clip, volume);
    }

    private static float EaseOutCubic(float x)
    {
        x = Mathf.Clamp01(x);
        float y = 1f - Mathf.Pow(1f - x, 3f);
        return y;
    }
}

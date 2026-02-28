using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Pools")]
    [SerializeField] private int sfxPoolSize = 10;

    private AudioSource musicSource;
    private List<AudioSource> sfxPool;
    private Dictionary<string, AudioSource> loopingSounds;

    [Header("Master Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Create music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Create SFX pool
        sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }

        loopingSounds = new Dictionary<string, AudioSource>();
    }

    #region Play Sounds
    /// <summary>
    /// Play a one-shot sound effect
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
    }

    /// <summary>
    /// Play a sound at a specific position in 3D space
    /// </summary>
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume * sfxVolume * masterVolume);
    }

    /// <summary>
    /// Play a looping sound with a unique identifier
    /// </summary>
    public void PlayLoopingSound(string id, AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || string.IsNullOrEmpty(id)) return;

        // Stop existing if playing
        if (loopingSounds.ContainsKey(id))
        {
            StopLoopingSound(id);
        }

        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.clip = clip;
            source.loop = true;
            source.volume = volume * sfxVolume * masterVolume;
            source.pitch = pitch;
            source.Play();
            loopingSounds[id] = source;
        }
    }

    public void PlayClipAtPosition(
    AudioClip clip,
    Vector3 position,
    float volume = 1f,
    float minPitch = 0.9f,
    float maxPitch = 1.1f)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.spatialBlend = 1f; // 3D sound

        audioSource.Play();

        Object.Destroy(tempGO, clip.length / Mathf.Abs(audioSource.pitch));
    }

    /// <summary>
    /// Play a looping 3D sound at a world position
    /// </summary>
    public void PlayLoopingSoundAtPosition(string id, AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || string.IsNullOrEmpty(id)) return;

        // Stop if already playing
        if (loopingSounds.ContainsKey(id))
        {
            StopLoopingSound(id);
        }

        GameObject soundObject = new GameObject($"LoopingSound_{id}");
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.volume = volume * sfxVolume * masterVolume;
        source.pitch = pitch;

        // 3D settings
        source.spatialBlend = 1f; // Fully 3D
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 1f;
        source.maxDistance = 20f;

        source.Play();

        loopingSounds[id] = source;
    }



    public void StopLoopingSound(string id)
    {
        if (loopingSounds.TryGetValue(id, out AudioSource source))
        {
            source.Stop();
            Destroy(source.gameObject);
            loopingSounds.Remove(id);
        }
    }

    /// <summary>
    /// Stop a looping sound by its identifier
    /// </summary>
    public void StopLoopingSoundAtPosition(string id)
    {
        if (loopingSounds.ContainsKey(id))
        {
            AudioSource source = loopingSounds[id];
            source.Stop();
            source.loop = false;
            source.clip = null;
            loopingSounds.Remove(id);
        }
    }

    /// <summary>
    /// Check if a looping sound is currently playing
    /// </summary>
    public bool IsLoopingSoundPlaying(string id)
    {
        return loopingSounds.ContainsKey(id) && loopingSounds[id].isPlaying;
    }

    /// <summary>
    /// Update parameters of a looping sound
    /// </summary>
    public void UpdateLoopingSound(string id, float? volume = null, float? pitch = null)
    {
        if (loopingSounds.ContainsKey(id))
        {
            AudioSource source = loopingSounds[id];
            if (volume.HasValue)
                source.volume = volume.Value * sfxVolume * masterVolume;
            if (pitch.HasValue)
                source.pitch = pitch.Value;
        }
    }
    #endregion

    #region Music
    /// <summary>
    /// Play background music
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true, float fadeInDuration = 0f)
    {
        if (clip == null || musicSource == null) return;

        if (fadeInDuration > 0f)
        {
            StartCoroutine(FadeMusic(clip, fadeInDuration, loop));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Stop the currently playing music
    /// </summary>
    public void StopMusic(float fadeOutDuration = 0f)
    {
        if (musicSource == null) return;

        if (fadeOutDuration > 0f)
        {
            StartCoroutine(FadeOutMusic(fadeOutDuration));
        }
        else
        {
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Pause the music
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    /// <summary>
    /// Resume the music
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip clip, float duration, bool loop)
    {
        // Fade out current music
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (duration / 2f));
            yield return null;
        }

        // Switch clips
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade in new music
        elapsed = 0f;
        float targetVolume = musicVolume * masterVolume;

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / (duration / 2f));
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private System.Collections.IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume * masterVolume;
    }
    #endregion

    #region Volume Controls
    /// <summary>
    /// Set master volume (affects all audio)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
    }

    /// <summary>
    /// Set SFX volume
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateLoopingSoundsVolume();
    }

    private void UpdateAllVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;

        UpdateLoopingSoundsVolume();
    }

    private void UpdateLoopingSoundsVolume()
    {
        foreach (var kvp in loopingSounds)
        {
            AudioSource source = kvp.Value;
            // Preserve the original volume ratio
            float originalVolume = source.volume / (sfxVolume * masterVolume);
            source.volume = originalVolume * sfxVolume * masterVolume;
        }
    }
    #endregion

    #region Utility
    /// <summary>
    /// Stop all sounds except music
    /// </summary>
    public void StopAllSounds()
    {
        foreach (AudioSource source in sfxPool)
        {
            source.Stop();
        }

        foreach (var kvp in loopingSounds)
        {
            kvp.Value.Stop();
        }
        loopingSounds.Clear();
    }

    /// <summary>
    /// Stop everything including music
    /// </summary>
    public void StopAllAudio()
    {
        StopAllSounds();
        musicSource?.Stop();
    }

    private AudioSource GetAvailableSFXSource()
    {

        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying && !source.loop)
            {
                return source;
            }
        }


        return sfxPool[0];
    }
    #endregion
}

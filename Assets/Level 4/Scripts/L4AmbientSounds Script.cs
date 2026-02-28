using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UIElements;
public class L4AmbientSoundsScript : MonoBehaviour
{
    [SerializeField] private List<AudioClip> AmbientSounds;
    [SerializeField] private List<Transform> SoundPositions;
    
    void Start()
    {
        AudioClip audio;
        Transform position;
        for (int i = 0; i < AmbientSounds.Count; i++)
        {
            audio = AmbientSounds[i];
            position = SoundPositions[i];
            AudioManager.Instance.PlayLoopingSoundAtPosition(audio.name, audio,position.position ,1, 1);
        }
    }

    
}

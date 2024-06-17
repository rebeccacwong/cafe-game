using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField]
    [Tooltip("Name of the sound")]
    private string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    [SerializeField]
    [Tooltip("Audio clip")]
    private AudioClip clip;
    public AudioClip Clip
    {
        get
        {
            return clip;
        }
    }

    [SerializeField]
    [Tooltip("Volume of audio clip, between 0 and 1.0.")]
    private float volume;
    public float Volume
    {
        get
        {
            if (volume == 0)
            {
                return 1.0f;
            }
            else
            {
                return volume;
            }
        }
    }

    [HideInInspector]
    private AudioSource source;
    public AudioSource Source
    {
        get
        {
            return source;
        }
        set
        {
            source = value;
        }
    }
}

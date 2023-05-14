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

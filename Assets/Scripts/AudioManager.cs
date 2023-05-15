using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Background music track")]
    private Sound[] musicTracks;

    [SerializeField]
    [Tooltip("Sound effects")]
    private Sound[] soundEffectSources;

    [SerializeField]
    [Tooltip("The audio mixer group specifically for background music")]
    private AudioMixerGroup musicGroup;

    [SerializeField]
    [Tooltip("The audio mixer group specifically for sound effects (foley)")]
    private AudioMixerGroup foleyGroup;
    #endregion

    #region Private Variables
    private Dictionary<String, Sound> soundEffects;
    private List<Sound> songs;
    private int m_indexOfCurrentlyPlayingSong = 0;
    #endregion

    #region Initialization
    private void Awake()
    {
        songs = new List<Sound>(); ;
        foreach (Sound s in musicTracks)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = musicGroup;
            songs.Add(s);
            //s.Source.loop = true;
        }
        Debug.LogWarning("playing " + songs[0].Name);
        songs[0].Source.Play();

        soundEffects = new Dictionary<string, Sound>();
        foreach (Sound s in soundEffectSources)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = foleyGroup;
            soundEffects.Add(s.Name, s);
        }
    }
    // void Start()
    // {
    //     Sound s = musicTracks[0];
    //     s.Source = gameObject.AddComponent<AudioSource>();
    //     s.Source.clip = s.Clip;
    //     s.Source.outputAudioMixerGroup = musicGroup;
    //     musicTracks[0].Source.loop = true;
    //     musicTracks[0].Source.Play();
    // }
    #endregion

    private void Update()
    {
        if (!songs[m_indexOfCurrentlyPlayingSong].Source.isPlaying)
        {
            m_indexOfCurrentlyPlayingSong = (m_indexOfCurrentlyPlayingSong + 1) % songs.Count;
            songs[m_indexOfCurrentlyPlayingSong].Source.Play();
            Debug.LogWarning("playing " + songs[m_indexOfCurrentlyPlayingSong].Name);
        }
    }

    #region Play Sound Methods
    public void StopBackgroundSong(int index)
    {
        songs[index % songs.Count].Source.Stop();
    }

    public void NextBackgroundSong(int index)
    {
        songs[index % songs.Count].Source.Play();
    }

    // public void StopTimeSong(int index) {
    // 	if (index < 0 || index >= musicTracks.Length) {
    // 		Debug.Log("Out of bounds: index of song should match time of day index.");
    // 	} else {
    // 		musicTracks[index].Source.Stop();
    // 	}
    // }

    // public void PlayTimeSong(int index) {
    // 	if (index < 0 || index >= musicTracks.Length) {
    // 		Debug.Log("Out of bounds: index of song should match time of day index.");
    // 	} else {
    // 		musicTracks[index].Source.Play();
    // 	}
    // }

    public void PlaySoundEffect(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with that name");
        }
        else
        {
            soundEffects[soundEffectName].Source.Play();
        }
    }

    public void PlaySoundEffectLoop(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with that name");
        }
        else
        {
            soundEffects[soundEffectName].Source.loop = true;
            soundEffects[soundEffectName].Source.Play();
        }
    }

    public void StopSoundEffectLoop(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with that name");
        }
        else
        {
            soundEffects[soundEffectName].Source.Stop();
        }
    }
    #endregion
}

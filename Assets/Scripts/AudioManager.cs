using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

/// <summary>
/// The AudioManager handles audio for all scenes.
/// There should only be one instance of this which
/// should persist between scenes.
///
/// The class has 2 mixers, one for the background music
/// and one for sound effects. 
/// </summary>

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
    private bool m_isMusicPaused = false;
    #endregion

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AudioManager is null!");
            }
            return _instance;
        }
    }

    #region Initialization
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        songs = new List<Sound>(); ;
        foreach (Sound s in musicTracks)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = musicGroup;
            s.Source.volume = s.Volume;
            songs.Add(s);
            //s.Source.loop = true;
        }
        Debug.Log("Playing " + songs[0].Name + " upon initialization of AudioManager.");
        songs[0].Source.Play();

        soundEffects = new Dictionary<string, Sound>();
        foreach (Sound s in soundEffectSources)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.outputAudioMixerGroup = foleyGroup;
            soundEffects.Add(s.Name, s);
        }

        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Update()
    {
        if (!m_isMusicPaused && !songs[m_indexOfCurrentlyPlayingSong].Source.isPlaying)
        {
            m_indexOfCurrentlyPlayingSong = (m_indexOfCurrentlyPlayingSong + 1) % songs.Count;
            songs[m_indexOfCurrentlyPlayingSong].Source.Play();
            Debug.Log("playing " + songs[m_indexOfCurrentlyPlayingSong].Name);
        }
    }

    #region Play Sound Methods
    public void PauseCurrentBackgroundSong()
    {
        songs[m_indexOfCurrentlyPlayingSong].Source.Stop();
        m_isMusicPaused = true;
    }

    public void ResumeCurrentBackgroundSong()
    {
        songs[m_indexOfCurrentlyPlayingSong].Source.Play();
        m_isMusicPaused = false;
    }

    public void NextBackgroundSong(int index)
    {
        songs[index % songs.Count].Source.Play();
    }

    public void PlaySoundEffect(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with name " + soundEffectName);
        }
        else
        {
            Sound sound = soundEffects[soundEffectName];
            sound.Source.PlayOneShot(sound.Clip, sound.Volume);
        }
    }

    public void PlaySoundEffectThenResumeBackgroundMusic(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with name " + soundEffectName);
        }
        else
        {
            PauseCurrentBackgroundSong();
            Sound sound = soundEffects[soundEffectName];
            sound.Source.Play();
            StartCoroutine(resumeBackgroundMusicWhenSoundDone(sound));
        }
    }

    private IEnumerator resumeBackgroundMusicWhenSoundDone(Sound sound)
    {
        while (sound.Source.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        ResumeCurrentBackgroundSong();
    }

    public void PlaySoundEffectLoop(string soundEffectName)
    {
        if (!soundEffects.ContainsKey(soundEffectName))
        {
            Debug.Log("Cant find the sound effect with name " + soundEffectName);
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
            Debug.Log("Cant find the sound effect with name " + soundEffectName);
        }
        else
        {
            soundEffects[soundEffectName].Source.Stop();
        }
    }
    #endregion
}

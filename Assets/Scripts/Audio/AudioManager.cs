using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages seperate playing of music and/or sfx from a central static class.
/// The AudioManager needs two seperate objects with AudioSource components on the scene: 
/// MusicPlayer and SfxPlayer, with their respective scripts attached. 
/// </summary>
public static class AudioManager
{
    // are scene objects initialized?
    static bool initialized = false;

    // script references on game objects in scene
    static List<MusicPlayer> musicPlayers = new List<MusicPlayer>();
    static SoundEffectsPlayer sfxPlayer;

    // AudioClip library
    public static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();


    public static SoundEffectsPlayer SfxPlayer { get => sfxPlayer; }
    public static bool Initialized { get => initialized; set => initialized = value; }

    /// <summary>
    /// Initializes sfx player object and populates AudioClip library.
    /// </summary>
    /// <param name="source">SoundEffectsPlayer script attached to the object</param>
    public static void Initialize(int musicChannelCount=1)
    {
        if (initialized)
        {
            return;
        }

        for (int i = 0; i < musicChannelCount; i++)
        {
            AddMusicPlayer();
        }

        sfxPlayer = AddSfxPlayer();

        foreach (AudioClipName clipName in Enum.GetValues(typeof(AudioClipName)))
        {
            string clipNameString = Enum.GetName(typeof(AudioClipName), clipName);
            AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + clipNameString);

            if (audioClip != null)
            {
                audioClips.TryAdd(clipName, audioClip);
            }
            else
            {
                Debug.LogWarning($"Audio file {clipName.ToString()} does not exist. Check file or enum spelling.");
            }
        }

        initialized = true;
    }

    public static bool HasMusicPlayer(int track)
    {
        return musicPlayers.Contains(musicPlayers[track]);
    }

    public static MusicPlayer GetMusicPlayer(int track)
    {
        return musicPlayers[track];
    }

    public static MusicPlayer AddMusicPlayer()
    {
        GameObject newPlayerObj = new GameObject($"MusicPlayer({musicPlayers.Count})");
        MusicPlayer musicPlayer = newPlayerObj.AddComponent<MusicPlayer>();
        musicPlayers.Add(musicPlayer);
        return musicPlayer;
    }

    private static SoundEffectsPlayer AddSfxPlayer()
    {
        GameObject newPlayerObj = new GameObject("SfxPlayer");
        SoundEffectsPlayer sfxPlayer = newPlayerObj.AddComponent<SoundEffectsPlayer>();
        return sfxPlayer;
    }

    /// <summary>
    /// Plays audio clip with the given name without looping.
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void PlaySfx(AudioClipName name, float volume=1f)
    {
        if (audioClips.ContainsKey(name))
        {
            sfxPlayer.AudioSource.PlayOneShot(audioClips[name], volume);
        }
    }

    /// <summary>
    /// Plays audio clip with the given name with looping enabled. 
    /// Use for music clips
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="volume"></param>
    public static void PlayMusic(int track, AudioClipName clipName, float volume=1f)
    {
        musicPlayers[track].AudioSource.Stop();
        
        if (!audioClips.ContainsKey(clipName))
        {
            Debug.LogWarning("Audio file {name} missing");
            return;
        }
        musicPlayers[track].AudioSource.clip = audioClips[clipName];
        musicPlayers[track].AudioSource.loop = true;
        musicPlayers[track].AudioSource.volume = volume;
        musicPlayers[track].AudioSource.Play();
    }

    /// <summary>
    /// Static wrapper function. Implemented in MusicPlayer script.
    /// </summary>
    public static void PlayMusicFadeIn(int track, AudioClipName clipName, float volume, float fadeDuration, float fadeDelay = 0)
    {
        musicPlayers[track].StopAllCoroutines();
        musicPlayers[track].AudioSource.loop = true;
        musicPlayers[track].PlayMusicFadeIn(audioClips[clipName], volume, fadeDuration, fadeDelay);
    }

    /// <summary>
    /// Static wrapper function. Implemented in MusicPlayer script.
    /// </summary>
    public static void FadeInMusic(int track, float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        musicPlayers[track].StopAllCoroutines();
        musicPlayers[track].StartCoroutine(musicPlayers[track].FadeInAudio(finalVolume, fadeDuration, fadeDelay));
    }

    /// <summary>
    /// Static wrapper function. Implemented in MusicPlayer script.
    /// </summary>
    public static void FadeOutMusic(int track, float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        musicPlayers[track].StopAllCoroutines();
        musicPlayers[track].StartCoroutine(musicPlayers[track].FadeOutAudio(finalVolume, fadeDuration, fadeDelay));
    }

    public static void StopMusic(int track)
    {
        musicPlayers[track].Stop();
    }
}

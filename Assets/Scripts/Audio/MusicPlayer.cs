using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for playing music with fade-in, fade-out options.
/// Attach this to a gameobject with AudioSource component.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    AudioSource audioSource;    // AudioSource component attached to this object.
    bool isPlaying;             // are we playing right now?

    public AudioSource AudioSource { get => audioSource; }
    public bool IsPlaying { get => GetComponent<AudioSource>().isPlaying; }

    void Awake()
	{
        // initialize audio manager and persist musicplayer object across scenes
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        
    }

    // Mute/Unmute music player
    public void Mute(bool isMuted)
    {
        audioSource.mute = isMuted;
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Starts music with fade in effect.
    /// </summary>
    /// <param name="clip"> AudioClip to play </param>
    /// <param name="volume"> Target volume to reach </param>
    /// <param name="fadeDuration"> Fade-in effect duration in seconds </param>
    /// <param name="startDelay"> Start after this many seconds </param>
    public void PlayMusicFadeIn(AudioClip clip, float volume, float fadeDuration, float startDelay = 0)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = 0;
        
        StartCoroutine(FadeInAudio(volume, fadeDuration, startDelay));
    }

    /// <summary>
    /// Coroutine that gradually increases music player volume.
    /// </summary>
    public IEnumerator FadeInAudio(float finalVolume, float fadeDuration, float startDelay=0)
    {
        float volume = audioSource.volume;
        yield return new WaitForSecondsRealtime(startDelay);

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        while (volume <= finalVolume)
        {
            // increment volume and pitch timer values
            volume += (Time.unscaledDeltaTime / (fadeDuration * 3));
            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Coroutine that gradually decreases music player volume.
    /// </summary>
    public IEnumerator FadeOutAudio(float finalVolume, float fadeDuration, float fadeDelay= 0)
    {
        float volume = audioSource.volume;
        yield return new WaitForSecondsRealtime(fadeDelay);

        while (volume >= finalVolume)
        {
            // increment volume and pitch timer values
            volume -= (Time.unscaledDeltaTime / fadeDuration);
            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }
}

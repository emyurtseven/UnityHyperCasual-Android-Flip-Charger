using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used for playing sound effects with fade-in, fade-out options.
/// Attach this to a gameobject with AudioSource component.
///
[RequireComponent(typeof(AudioSource))]
public class SoundEffectsPlayer : MonoBehaviour
{
    AudioSource audioSource;    // AudioSource component attached to this object.

    public AudioSource AudioSource { get => audioSource; }

    void Awake()
	{
        // initialize audio manager and persist sfx player object across scenes
        if (!AudioManager.Initialized)
        {
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Coroutine that gradually increases music player volume.
    /// </summary>
    public IEnumerator FadeInAudio(float finalVolume, float fadeDuration, float startDelay=0)
    {
        float volume = audioSource.volume;
        yield return new WaitForSeconds(startDelay);

        audioSource.Play();

        while (volume <= finalVolume)
        {
            // increment volume and pitch timer values
            volume += (Time.deltaTime / fadeDuration);
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
        yield return new WaitForSeconds(fadeDelay);

        while (volume >= finalVolume)
        {
            // increment volume and pitch timer values
            volume -= (Time.deltaTime / fadeDuration);
            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }
}

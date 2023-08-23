using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shakes camera for visual effect. Attached to main camera.
/// </summary>
public class CameraShake : MonoBehaviour
{
    private void Start() 
    {
        EventManager.AddDoubleFloatArgumentListener(CameraShakeEventListener, EventType.CameraShake);
    }

    /// <summary>
    /// Listens for camera shake event. The event is invoked in player collision script. 
    /// </summary>

    public void CameraShakeEventListener(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    /// <summary>
    /// Shakes camera for the given duration.
    /// </summary>
    /// <param name="duration"> Duration of the effect </param>
    /// <param name="magnitude"> Magnitude of camera movement </param>
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, -10f);
            elapsed += Time.unscaledDeltaTime;
            yield return 0;
        }
        transform.position = orignalPosition;
    }
}

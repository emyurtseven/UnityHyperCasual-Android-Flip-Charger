using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : Invoker
{
    [SerializeField] GameObject impactParticles;

    DoubleFloatArgumentEvent cameraShakeEvent = new DoubleFloatArgumentEvent();

    private void Start()
    {
        doubleFloatArgEventDict.Add(EventType.CameraShake, cameraShakeEvent);
        EventManager.AddDoubleFloatArgumentInvoker(this, EventType.CameraShake);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.DebugMode)
        {
            (float duration, float magnitude) = GameManager.Instance.CameraShakeParameters;

            InvokeDoubleFloatArgEvent(EventType.CameraShake, duration, magnitude);
                                        
            Instantiate(impactParticles, transform.position, Quaternion.identity);
            AudioManager.PlaySfx(AudioClipName.Impact);
            GameManager.Instance.LoseLife();
        }
    }
}

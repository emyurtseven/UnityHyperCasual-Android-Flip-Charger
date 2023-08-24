using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float[] playerYPositions = new float[2];
    [SerializeField] float playerFlipSpeed = 10f;

    int playerCurrentLane = 0;

    ParticleSystem trailParticles;

    bool controlsActive = true;

    public bool ControlsActive { get => controlsActive; set => controlsActive = value; }

    private void Start() 
    {
        trailParticles = GetComponent<ParticleSystem>();
        EventManager.AddNoArgumentListener(SetParticleSpeed, EventType.DifficultyChanged);

        SetParticleSpeed();
    }

    private void OnFlip()
    {
        if (controlsActive)
        {
            playerCurrentLane = (playerCurrentLane + 1) % 2;
            Vector2 targetPos = new Vector2(transform.position.x, playerYPositions[playerCurrentLane]);
            StartCoroutine(LerpPosition(targetPos, 1f / playerFlipSpeed));
        }
    }

    public void ResetPosition()
    {
        playerCurrentLane = 0;
        transform.position = new Vector2(transform.position.x, playerYPositions[playerCurrentLane]);
    }

    private void SetParticleSpeed()
    {
        var main = trailParticles.main;
        main.startSpeed = GameManager.Instance.GameSpeed;
    }

    IEnumerator LerpPosition(Vector2 targetPosition, float duration)
    {
        controlsActive = false;
        float time = 0;
        Vector2 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        controlsActive = true;
    }
}

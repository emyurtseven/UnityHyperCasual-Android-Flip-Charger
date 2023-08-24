using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Manages gameplay.
/// IMPORTANT: This script needs to be loaded before default time.
/// </summary>
public class GameManager : Invoker
{
    [Header("Default starting values")]
    [SerializeField] float gameSpeedStart = 10f;
    [SerializeField] float startingMinSpawnInterval = 1.5f;
    [SerializeField] float startingMaxSpawnInterval = 2f;

    [Header("Difficulty settings")]
    [SerializeField] int playerMaxLives = 3;
    [SerializeField] int difficultyUpThreshold = 15;
    [Tooltip("GameSpeed increases by this amount upon difficulty up")]
    [SerializeField] float gameSpeedIncrement = 1f;
    [Tooltip("Min spawn interval decreases by this amount upon difficulty up")]
    [SerializeField] float minSpawnDecrement = 0.1f;
    [Tooltip("Max spawn interval decreases by this amount upon difficulty up")]
    [SerializeField] float maxSpawnDecrement = 0.2f;

    [Header("Visual settings")]
    [SerializeField] float cameraShakeDuration = 0.5f;
    [SerializeField] float cameraShakeMagnitude = 0.5f;

    [SerializeField] ParticleSystem backgroundParticles;
    [SerializeField] bool debugMode = false;

    // listeners for this are set in editor instead of scripts
    NoArgumentEvent difficultyUpEvent = new NoArgumentEvent();
    NoArgumentEvent gameOverEvent = new NoArgumentEvent();

    int highScore;

    int playerCurrentLives;
    int scoreCurrent = 0;
    float gameSpeedCurrent;
    float minIntervalCurrent;
    float maxIntervalCurrent;

    GameObject player;
    PlayerMovement playerMovement;
    ParticleSystem playerParticles;

    public static GameManager Instance { get; private set; }

    public float GameSpeed { get => gameSpeedCurrent; }
    public (float, float) SpawnIntervals { get => (minIntervalCurrent, maxIntervalCurrent); }
    public (float, float) CameraShakeParameters { get => (cameraShakeDuration, cameraShakeMagnitude); }
    public bool DebugMode { get => debugMode; }
    public int PlayerMaxLives { get => playerMaxLives; }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerParticles = player.GetComponent<ParticleSystem>();

        if (Instance == null)
        {
            Instance = this;
        }

        ObjectPool.Initialize();
        AudioManager.Initialize();
        EventManager.Initialize();
    }

    private void Start() 
    {
        noArgEventDict.Add(EventType.DifficultyChanged, difficultyUpEvent);
        EventManager.AddNoArgumentInvoker(this, EventType.DifficultyChanged);

        noArgEventDict.Add(EventType.GameOver, gameOverEvent);
        EventManager.AddNoArgumentInvoker(this, EventType.GameOver);

        gameSpeedCurrent = gameSpeedStart;
        minIntervalCurrent = startingMinSpawnInterval;
        maxIntervalCurrent = startingMaxSpawnInterval;

        LoadHighScore();
        UIManager.Instance.DisplayHighScore(highScore);
        
        playerMovement.ControlsActive = false;
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        playerParticles.Play();
        backgroundParticles.Play();
        AudioManager.PlayMusic(0, AudioClipName.PlayerMovement, 0.5f);
        Time.timeScale = 1;

        playerCurrentLives = playerMaxLives;
        scoreCurrent = 0;

        UIManager.Instance.DisplayScore(scoreCurrent);
        UIManager.Instance.DisplayLives(playerCurrentLives);

        playerMovement.ControlsActive = true;
    }

    /// <summary>
    /// Checks if threshold number of obstacles have been cleared.
    /// </summary>
    private void CheckDifficultyUp()
    {
        if (scoreCurrent % difficultyUpThreshold == 0)
        {
            gameSpeedCurrent += gameSpeedIncrement;
            minIntervalCurrent = Mathf.Max(0.2f, minIntervalCurrent - minSpawnDecrement);
            maxIntervalCurrent = Mathf.Max(0.4f, maxIntervalCurrent - maxSpawnDecrement);

            InvokeNoArgumentEvent(EventType.DifficultyChanged);
        }
    }

    private void LoadHighScore()
    {
        if (PlayerPrefs.HasKey("highScore"))
        {
            highScore = PlayerPrefs.GetInt("highScore");
        }
        else
        {
            highScore = 0;
        }

        UIManager.Instance.DisplayHighScore(highScore);
    }

    public void ScoreUp()
    {
        scoreCurrent++;
        UIManager.Instance.DisplayScore(scoreCurrent);

        CheckDifficultyUp();
        SaveScore();
    }


    public void LoseLife()
    {
        playerCurrentLives--;
        UIManager.Instance.DisplayLives(playerCurrentLives);
        
        if (playerCurrentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        InvokeNoArgumentEvent(EventType.GameOver);

        Time.timeScale = 0;
        playerMovement.ControlsActive = false;
        playerParticles.Pause();
        AudioManager.StopMusic(0);

        SaveScore();
    }

    /// <summary>
    /// We don't reset the scene, just despawn obstacles and start spawning again.
    /// </summary>
    public void RestartGame()
    {
        playerMovement.ControlsActive = true;
        playerMovement.ResetPosition();

        gameSpeedCurrent = gameSpeedStart;
        minIntervalCurrent = startingMinSpawnInterval;
        maxIntervalCurrent = startingMaxSpawnInterval;

        InvokeNoArgumentEvent(EventType.DifficultyChanged);

        ObjectPool.ReturnAllPooledObjects();    // return currently active obstacles to pool

        StartGame();
    }
    
    private void SaveScore()
    {
        if (scoreCurrent > highScore)
        {
            PlayerPrefs.SetInt("highScore", scoreCurrent);
        }
    }

    private void OnApplicationQuit()
    {
        SaveScore();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        SaveScore();
    }
}

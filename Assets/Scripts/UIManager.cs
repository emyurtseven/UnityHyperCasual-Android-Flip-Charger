using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Manages UI elements and functionalities.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject HUDPanel;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] GameObject livesIconsContainer;
    [SerializeField] GameObject speedIconsContainer;

    [SerializeField] GameObject lifeIcon;
    [SerializeField] GameObject speedIcon;

    Color speedBarDefaultColor;

    int speed = 1;

    public static UIManager Instance { get; private set; }

    private void Awake() 
    {
        // add a static access to this class
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start() 
    {
        speedBarDefaultColor = speedIcon.GetComponent<Image>().color;
        EventManager.AddNoArgumentListener(IncreaseSpeed, EventType.DifficultyChanged);
        EventManager.AddNoArgumentListener(OnGameOver, EventType.GameOver);
    }

    /// <summary>
    /// Callback for start button
    /// </summary>
    public void OnStartClicked()
    {
        startPanel.SetActive(false);
        HUDPanel.SetActive(true);
        DisplaySpeed();

        GameManager.Instance.StartGame();
    }

    /// <summary>
    /// Callback for retry button
    /// </summary>
    public void OnRetryClicked()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.RestartGame();

        speed = 1;
        DisplaySpeed();
    }

    private void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void DisplayHighScore(int highScore)
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    public void DisplaySpeed()
    {
        float alpha = 25f;

        foreach (Transform child in speedIconsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < speed; i++)
        {
            Image barIcon = Instantiate(speedIcon, speedIconsContainer.transform).GetComponent<Image>();
            barIcon.color = new Color(speedBarDefaultColor.r, speedBarDefaultColor.g, 
                                        speedBarDefaultColor.b, alpha / 255f);
            alpha += 12f;
        }
    }

    private void IncreaseSpeed()
    {
        speed++;
        DisplaySpeed();
    }

    public void DisplayScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void DisplayLives(int lives)
    {
        foreach (Transform child in livesIconsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < lives; i++)
        {
            Instantiate(lifeIcon, livesIconsContainer.transform);
        }
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}

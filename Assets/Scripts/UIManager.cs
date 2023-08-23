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
    [SerializeField] GameObject livesPanel;

    [SerializeField] GameObject lifeIcon;


    public static UIManager Instance { get; private set; }

    private void Awake() 
    {
        // add a static access to this class
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Callback for start button
    /// </summary>
    public void OnStartClicked()
    {
        startPanel.SetActive(false);
        HUDPanel.SetActive(true);

        GameManager.Instance.StartGame();
    }

    /// <summary>
    /// Callback for retry button
    /// </summary>
    public void OnRetryClicked()
    {
        gameOverPanel.SetActive(false);

        GameManager.Instance.RestartGame();
    }

    public void OnGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void DisplayHighScore(int highScore)
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    public void DisplayScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void DisplayLives(int lives)
    {
        foreach (Transform child in livesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < lives; i++)
        {
            Instantiate(lifeIcon, livesPanel.transform);
        }
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}

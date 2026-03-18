using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Allows Targets to find this easily

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Settings")]
    [SerializeField] private float timeRemaining = 180f;
    [SerializeField] private int targetScore = 50;

    private int currentScore = 0;
    private bool gameEnded = false;

    private void Awake()
    {
        // Singleton pattern: ensures only one GameManager exists
        Instance = this;
    }

    private void Update()
    {
        if (gameEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            EndGame("Time's Up! Game Over.");
        }
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        currentScore += amount;
        scoreText.text = "Score: " + currentScore;

        if (currentScore >= targetScore)
        {
            EndGame("Target Reached! You Win!");
        }
    }

    private void UpdateTimerUI()
    {
        // Formats time to 00:00 style
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void EndGame(string message)
    {
        gameEnded = true;
        Debug.Log(message);
        Time.timeScale = 0;
    }
}
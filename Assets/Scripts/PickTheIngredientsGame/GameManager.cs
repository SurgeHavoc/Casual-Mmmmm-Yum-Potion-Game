using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private int TotalIngredients;
    private int CollectedIngredients = 0;

    public float TimeLimit = 60f;
    private float TimeRemaining;
    public TextMeshProUGUI TimerText;
    private bool IsTimerRunning = false;

    public GameObject GameOverPanel;
    public GameObject GameCompletePanel;

    public bool IsGameOver = false;

    public ClawHandController ClawHandControllerInstance;

    public Button RestartButton;
    public Button MainMenuButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get total ingredients at the start.
        TotalIngredients = GameObject.FindGameObjectsWithTag("Ingredient").Length;

        TimeRemaining = TimeLimit;
        UpdateTimerDisplay();

        IsTimerRunning = true;

        if(GameOverPanel != null)
        {
            GameOverPanel.SetActive(false);
        }

        if (RestartButton != null)
            RestartButton.gameObject.SetActive(false);

        if (MainMenuButton != null)
            MainMenuButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
            return;

        if (IsTimerRunning)
        {
            TimeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                IsTimerRunning = false;
                UpdateTimerDisplay();
                GameOver();
            }

            else if (TimeRemaining <= 5)
            {
                // Start the blinking timer when time is running out to complete the minigame.
                BlinkTimer();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (TimerText != null)
        {
            int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt((TimeRemaining * 1000f) % 1000f);

            TimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            TimerText.color = Color.white;
        }
    }

    void BlinkTimer()
    {
        if (TimerText != null)
        {
            TimerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 2, 1));
        }
    }

    public void IngredientCollected()
    {
        CollectedIngredients++;
        if (CollectedIngredients >= TotalIngredients)
        {
            Debug.Log("All ingredients collected! Minigame complete!");
            GameComplete();
        }
    }

    void GameOver()
    {
        IsGameOver = true;
        IsTimerRunning = false;

        // On game over, disable input for claw hand.
        if (ClawHandControllerInstance != null)
        {
            ClawHandControllerInstance.DisableInput();
        }

        // On game over, show game over UI.
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);

            TextMeshProUGUI GameOverText = GameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (GameOverText != null)
            {
                GameOverText.text = "Game Over";
            }
        }

        // On game over, enable game over buttons.
        RestartButton.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(true);

        RestartButton.onClick.RemoveAllListeners();
        MainMenuButton.onClick.RemoveAllListeners();

        RestartButton.onClick.AddListener(RestartGame);
        MainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void RestartGame()
    {
        // Reload the current scene here.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // Load the MenuScene here.
        SceneManager.LoadScene("MenuScene");
    }

    void GameComplete()
    {
        IsGameOver = true;
        IsTimerRunning = false;

        // On minigame complete, disable input for claw hand.
        if (ClawHandControllerInstance != null)
        {
            ClawHandControllerInstance.DisableInput();
        }

        // On minigame complete, show game complete UI.
        if (GameCompletePanel != null)
        {
            GameCompletePanel.SetActive(true);

            TextMeshProUGUI GameCompleteText = GameCompletePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (GameCompleteText != null)
            {
                GameCompleteText.text = "Minigame complete!";
            }

            //RestartButton.gameObject.SetActive(true);
            //MainMenuButton.gameObject.SetActive(true);

            //RestartButton.onClick.RemoveAllListeners();
            //MainMenuButton.onClick.RemoveAllListeners();

            //RestartButton.onClick.AddListener(ProceedToNextMinigame);
            //MainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    void ProceedToNextMinigame()
    {
        Debug.Log("Proceed to the next minigame!");
    }

    private void OnDisable()
    {
        if (RestartButton != null)
        {
            RestartButton.onClick.RemoveAllListeners();
        }
        if (MainMenuButton != null)
        {
            MainMenuButton.onClick.RemoveAllListeners();
        }
    }
}

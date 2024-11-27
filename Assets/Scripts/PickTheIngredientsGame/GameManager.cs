using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private int TotalIngredients;
    private int CollectedIngredients = 0;

    public float TimeLimit = 25f;
    private float TimeRemaining;
    public TextMeshProUGUI TimerText;
    public bool IsTimerRunning = false;

    public GameObject GameOverPanel;
    public GameObject GameCompletePanel;

    public bool IsGameOver = false;

    public ClawHandController ClawHandControllerInstance;

    public Button RestartButton;
    public Button MainMenuButton;

    public PauseScript pause; // PauseMenu
    public GameObject failPopUp;
    public GameObject instruct; // hi
    public GameObject successPopUp;

    public Ingredient IngredientPrefab;
    public List<Sprite> AvailableIngredientSprites;
    public List<Transform> IngredientSpawnPoints;

    public Canvas GameCanvas;

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

        // Allows the Pause Menu to function -Asha
        pause = GetComponent<PauseScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Intro();

        // Get total ingredients at the start.
        TotalIngredients = GameObject.FindGameObjectsWithTag("Ingredient").Length;

        TimeRemaining = TimeLimit;
        UpdateTimerDisplay();

        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(false);
        }

        if (RestartButton != null)
            RestartButton.gameObject.SetActive(false);

        if (MainMenuButton != null)
            MainMenuButton.gameObject.SetActive(false);

        SpawnRandomIngredients();

        TotalIngredients = 3;
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
                failPopUp.SetActive(true); // Fail PopUp
            }

            else if (TimeRemaining <= 5)
            {
                // Start the blinking timer when time is running out to complete the minigame.
                BlinkTimer();
            }
        }

        // Hey Antonio this was the only way I could get the pause menu to work pls don't delete -Asha
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!pause.GameIsPaused)
                pause.Paused();
            else
                pause.Resume();
        }
    }

    public void Intro() // coding for Instructions popup
    {
        instruct.SetActive(true);
        pause.GameIsPaused = true;
        IsTimerRunning = false;
        Time.timeScale = 0;
    }

    void UpdateTimerDisplay()
    {
        if (TimerText != null)
        {
            //int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            //int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            //int milliseconds = Mathf.FloorToInt((TimeRemaining * 1000f) % 1000f);

            //TimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            TimerText.text = "Time: " + Mathf.CeilToInt(TimeRemaining).ToString();
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

    void SpawnRandomIngredients()
    {
        // Make sure that there are enough spawn points.
        if (IngredientSpawnPoints.Count < 3)
        {
            Debug.LogError("Not enough spawn points for the ingredients!");
            return;
        }

        List<Sprite> ShuffledSprites = new List<Sprite>(AvailableIngredientSprites);
        ShuffleList(ShuffledSprites);

        List<Sprite> SelectedSprites = ShuffledSprites.GetRange(0, 3);

        List<Transform> ShuffledSpawnPoints = new List<Transform>(IngredientSpawnPoints);
        ShuffleList(ShuffledSpawnPoints);

        for (int i = 0; i < 3; i++)
        {
            SpawnIngredient(SelectedSprites[i], ShuffledSpawnPoints[i].localPosition);
        }
    }

    void SpawnIngredient(Sprite sprite, Vector3 position)
    {
        Ingredient NewIngredient = Instantiate(IngredientPrefab, GameCanvas.transform);
        NewIngredient.SetIngredientSprite(sprite);
        NewIngredient.gameObject.tag = "Ingredient";

        RectTransform IngredientRect = NewIngredient.GetComponent<RectTransform>();
        IngredientRect.anchoredPosition = position;
    }

    // This uses the Fisher-Yates shuffle algorithm to randomize the order of items in a list.
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int RandomIndex = Random.Range(i, list.Count);
            list[i] = list[RandomIndex];
            list[RandomIndex] = temp;
        }
    }


    public void IngredientCollected()
    {
        CollectedIngredients++;
        if (CollectedIngredients >= TotalIngredients)
        {
            Debug.Log("All ingredients collected! Minigame complete!");
            GameComplete();
            successPopUp.SetActive(true);
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
        //RestartButton.gameObject.SetActive(true);
        //MainMenuButton.gameObject.SetActive(true);

        //RestartButton.onClick.RemoveAllListeners();
        //MainMenuButton.onClick.RemoveAllListeners();

        //RestartButton.onClick.AddListener(RestartGame);
        //MainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void RestartGame()
    {
        // Reload the current scene here.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        CollectedIngredients = 0;
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

            RestartButton.gameObject.SetActive(false);
            MainMenuButton.gameObject.SetActive(false);

            //RestartButton.gameObject.SetActive(true);
            //MainMenuButton.gameObject.SetActive(true);

            //RestartButton.onClick.RemoveAllListeners();
            //MainMenuButton.onClick.RemoveAllListeners();

            //RestartButton.onClick.AddListener(ProceedToNextMinigame);
            //MainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }

        Invoke("ProceedToNextMinigame", 3f);
    }

    void ProceedToNextMinigame()
    {
        Debug.Log("Proceed to the next minigame!");

        GameCompletePanel.SetActive(false);

        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int NextSceneIndex = CurrentSceneIndex + 1;

        if (NextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(NextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes. Maybe pause the game and exit?");
        }
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
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;

public class DancingSeedGame : MonoBehaviour
{
    public GameObject KeyPromptPrefab;
    public Transform KeyPromptPanel;
    public SpriteRenderer PlayerRenderer;
    public int TotalRounds = 3; // Can be up to five!

    public Sprite IdleSprite;
    public Sprite LeftPoseSprite;
    public Sprite UpPoseSprite;
    public Sprite UpStinkyPoseSprite;
    public Sprite DownPoseSprite;
    public Sprite RightPoseSprite;

    public PauseScript pause; // PauseMenu

    private List<string> colors = new List<string> { "Red", "Blue", "Green", "Yellow" };
    private Dictionary<string, KeyCode> ColorKeyMap = new Dictionary<string, KeyCode>
    {
        { "Yellow", KeyCode.UpArrow },
        { "Green", KeyCode.DownArrow },
        { "Blue", KeyCode.LeftArrow },
        { "Red", KeyCode.RightArrow }
    };
    private Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
    {
        { "Red", Color.red },
        { "Blue", Color.blue },
        { "Green", Color.green },
        { "Yellow", Color.yellow }
    };

    private List<string> CurrentSequence = new List<string>();
    private int CurrentInputIndex = 0;
    private int CurrentRound = 0;
    private bool IsInputEnabled = false;

    private PlayerInputActions InputActions;

    // Variables for the timer.
    public float TimeLimit = 20f; // Sets the timer duration.
    private float TimeRemaining;
    public TextMeshProUGUI TimerText;
    private bool IsTimerRunning = false;

    // Round complete UI.
    public TextMeshProUGUI RoundCompleteText;
    public float RoundCompleteDisplayTime = 2f;

    // Game start countdown UI.
    public TextMeshProUGUI CountdownText;
    public float CountdownInterval = 1f; // The time between each number in the countdown.

    // Game over UI.
    public TextMeshProUGUI GameOverText;

    private void Awake()
    {
        InputActions = new PlayerInputActions();

        InputActions.DancingSeedPlayer.MoveUp.performed += ctx => OnMove("Yellow");
        InputActions.DancingSeedPlayer.MoveDown.performed += ctx => OnMove("Green");
        InputActions.DancingSeedPlayer.MoveLeft.performed += ctx => OnMove("Blue");
        InputActions.DancingSeedPlayer.MoveRight.performed += ctx => OnMove("Red");

        // Allows the Pause Menu to function -Asha
        pause = GetComponent<PauseScript>();
    }

    private void OnEnable()
    {
        InputActions.DancingSeedPlayer.Enable();
    }

    private void OnDisable()
    {
        InputActions.DancingSeedPlayer.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the timer here.
        TimeRemaining = TimeLimit;
        UpdateTimerDisplay();

        // Begin the countdown on game start here.
        StartCountdown();
    }

    void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        // Disable input and pause the timer during the countdown.
        IsInputEnabled = false;
        IsTimerRunning = false;

        if(CountdownText != null)
        {
            CountdownText.gameObject.SetActive(true);
        }

        string[] CountdownNumbers = { "3", "2", "1", "DANCE!" };

        // Variable to limit the total countdown duration.
        float TotalCountdownDuration = 3f;

        float TimePerNumber = TotalCountdownDuration / CountdownNumbers.Length; // 3s / 4 = 0.75s per number

        foreach (string number in CountdownNumbers)
        {
            if(CountdownText != null)
            {
                CountdownText.text = number;

                Color TextColor = CountdownText.color;
                TextColor.a = 1f;
                CountdownText.color = TextColor;

                // Reset scale to <3, 3, 3>.
                CountdownText.rectTransform.localScale = new Vector3(3, 3, 3);
            }

            float EffectDuration= TimePerNumber * 0.99f; // Fade over 99% of the time per number.
            float ElapsedTime = 0f;

            while (ElapsedTime < EffectDuration)
            {
                ElapsedTime += Time.deltaTime;
                float t = ElapsedTime / EffectDuration;

                // Smooths the interpolation.
                float EasedT = Mathf.SmoothStep(0f, 1f, t);

                // Calculate the alpha fading from 1 to 0.
                float alpha = Mathf.Lerp(1f, 0f, EasedT);
                // Calculate the scale from <3, 3, 3> to <0, 0, 0>.
                float scale = Mathf.Lerp(3f, 0.5f, EasedT);

                Color StartColor = Color.white;
                Color EndColor = Color.gray;

                // Fun rotation stuff here.
                //float rotation = Mathf.Lerp(0f, 360f, EasedT);

                if (CountdownText != null)
                {
                    Color TextColor = CountdownText.color;
                    TextColor.a = alpha;
                    CountdownText.color = TextColor;

                    CountdownText.rectTransform.localScale = new Vector3(scale, scale, scale);

                    CountdownText.color = Color.Lerp(StartColor, EndColor, EasedT);

                    // Fun rotation stuff.
                    //CountdownText.rectTransform.rotation = Quaternion.Euler(0f, 0f, rotation);
                }
                yield return null;
            }

            if (CountdownText != null)
            {
                Color TextColor = CountdownText.color;
                TextColor.a = 0f;
                CountdownText.color = TextColor;

                CountdownText.rectTransform.localScale = Vector3.zero;
            }

            // A small delay before the next number (if any).
            float delay = TimePerNumber - EffectDuration;
            if(delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
        }

        if(CountdownText != null)
        {
            CountdownText.gameObject.SetActive(false);
        }

        // After the countdown is finished, start the first round.
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsInputEnabled)
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

        // Hey Antonio this was the only way I could get the pause menu to work pls don't delete -Asha
         if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7)) 
        {
            if(!pause.GameIsPaused)
                    pause.Paused();
                else
                    pause.Resume();
        }

        if (Input.anyKeyDown)
        {
            foreach (var ColorName in colors)
            {
                if (Input.GetKeyDown(ColorKeyMap[ColorName]))
                {
                    CheckInput(ColorName);
                    ChangeSeedPose(ColorName);
                    break;
                }
            }
        }
    }

    private void OnMove(string ColorName)
    {
        Debug.Log("OnMove called with ColorName " + ColorName);
        if (!IsInputEnabled)
            return;

        //ChangeSeedPose(ColorName);

        CheckInput(ColorName);
    }

    private void ChangeSeedPose(string ColorName)
    {
        Debug.Log("ChangeSeedPose called with ColorName " + ColorName);
        Sprite NewSprite = IdleSprite;

        switch(ColorName)
        {
            case "Red":
                NewSprite = RightPoseSprite;
                break;
            case "Blue":
                NewSprite = LeftPoseSprite;
                break;
            case "Green":
                NewSprite = DownPoseSprite;
                break;
            case "Yellow":
                float chance = Random.Range(0f, 1f);
                if (chance < 0.4f)
                {
                    NewSprite = UpStinkyPoseSprite;
                    Debug.Log("UpStinkyPoseSprite selected with chance: " + chance);
                }
                else
                {
                    NewSprite = UpPoseSprite;
                    Debug.Log("UpPoseSprite selected with chance: " + chance);
                }
                break;
            default:
                Debug.LogWarning("Unknown ColorName: " + ColorName);
                break;
        }

        PlayerRenderer.sprite = NewSprite;
        Debug.Log("Changed sprite to " + NewSprite.name);

        CancelInvoke("ResetSeedPose");
        Invoke("ResetSeedPose", 1.5f);
    }

    private void ResetSeedPose()
    {
        PlayerRenderer.sprite = IdleSprite;
    }

    void StartRound()
    {
        // Reset the player color on round start.
        PlayerRenderer.color = Color.white;

        // Clear all key prompts on round start.
        foreach (Transform child in KeyPromptPanel)
        {
            Destroy(child.gameObject);
        }

        // Generate a random sequence length between 3 and 5 (6 is exclusive) on round start.
        int SequenceLength = Random.Range(3, 6);

        CurrentSequence.Clear();
        // Generate a random sequence on round start.
        for (int i = 0; i < SequenceLength; i++)
        {
            string ColorName = colors[Random.Range(0, colors.Count)];
            CurrentSequence.Add(ColorName);
        }

        CurrentInputIndex = 0;

        // Display the first prompt here.
        DisplayCurrentPrompt();

        //TimerText.color = Color.white;
        UpdateTimerDisplay();


        // Input is now enabled.
        IsInputEnabled = true;
        // Timer is now enabled.
        IsTimerRunning = true;
    }

    void DisplayCurrentPrompt()
    {
        foreach (Transform child in KeyPromptPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the current prompt UI.
        string ColorName = CurrentSequence[CurrentInputIndex];
        GameObject prompt = Instantiate(KeyPromptPrefab, KeyPromptPanel);
        Image PromptImage = prompt.GetComponent<Image>();
        if (PromptImage != null)
        {
            PromptImage.color = ColorMap[ColorName];
        }
    }

    void CheckInput(string InputColor)
    {
        if(InputColor == CurrentSequence[CurrentInputIndex])
        {
            // If input is correct, change the player accordingly.
            //PlayerRenderer.color = ColorMap[InputColor];

            CurrentInputIndex++;

            if(CurrentInputIndex >= CurrentSequence.Count)
            {
                // On round complete, increment round.
                CurrentRound++;

                // If all rounds are successfully completed, game is complete. Else, start a new round.
                if(CurrentRound >= TotalRounds)
                {
                    CompleteGame();
                }
                else
                {
                    RoundCompleted();
                }
            }
            else
            {
                DisplayCurrentPrompt();
            }
        }
        else
        {
            // On incorrect input.
            Debug.Log("Incorrect input. Try again.");

            // On incorrect input, change the player color to indicate an error, oh noes!
            //PlayerRenderer.color = Color.black;

            IsInputEnabled = false;
            // Restart the round after a delay.
            Invoke("StartRound", 1f);
        }
    }

    void RoundCompleted()
    {
        // Disable input here.
        IsInputEnabled = false;
        // Pause the timer here.
        IsTimerRunning = false;

        if (RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(true);
            RoundCompleteText.text = $"Round {CurrentRound} Completed!";
        }

        // Start the next round after waiting for RoundCompleteDisplayTime.
        Invoke("StartNextRound", RoundCompleteDisplayTime);
    }

    void StartNextRound()
    {
        if(RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(false);
        }

        // Placeholder for if countdown is needed before each round.
        // StartCountdown();

        // StartCountdown(); is used, comment out StartRound(); below.
        StartRound();
    }

    void UpdateTimerDisplay()
    {
        if(TimerText != null)
        {
            int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt((TimeRemaining * 1000f) % 1000f);

            TimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            // TimerText.text = "Time: " + Mathf.CeilToInt(TimeRemaining).ToString();
            TimerText.color = Color.white;
        }
    }

    void BlinkTimer()
    {
        if (TimerText != null)
        {
            // Make TimerText blink by changing its color.
            TimerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 2, 1));
        }
    }

    void GameOver()
    {
        // Disable the input.
        IsInputEnabled = false;
        // Disable the timer.
        IsTimerRunning = false;

        if(GameOverText != null)
        {
            GameOverText.gameObject.SetActive(true);
            GameOverText.text = "GAME OVER!";
        }

        Invoke("RestartGame", 3f);
    }

    void RestartGame()
    {
        // Reset the minigame variables.
        CurrentRound = 0;
        CurrentInputIndex = 0;

        // Reset the timer.
        TimeRemaining = TimeLimit;
        IsTimerRunning = true;

        if(RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(false);
        }

        if(GameOverText != null)
        {
            GameOverText.gameObject.SetActive(false);
        }

        StartRound();
    }

    void CompleteGame()
    {
        Debug.Log("Game Complete!");

        PlayerRenderer.color = Color.white;

        IsInputEnabled = false;

        IsTimerRunning = false;

        if(RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(true);
            RoundCompleteText.text = "Game Complete!";
        }

        Invoke("ProceedToNextMinigame", 2f);
    }

    void ProceedToNextMinigame()
    {
        if (RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(false);
        }

        Debug.Log("Proceeding to the next minigame.");
    }
}

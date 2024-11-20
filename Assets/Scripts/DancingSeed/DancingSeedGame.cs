using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.SceneManagement;

public class DancingSeedGame : MonoBehaviour
{
    public GameObject ArrowPromptPrefab;
    public Transform KeyPromptPanel;
    public SpriteRenderer PlayerRenderer;
    public int TotalRounds = 3; // Can be up to five!

    public Sprite IdleSprite;
    public Sprite LeftPoseSprite;
    public Sprite UpPoseSprite;
    public Sprite UpStinkyPoseSprite;
    public Sprite DownPoseSprite;
    public Sprite RightPoseSprite;
    public Sprite TransformationSprite;

    public PauseScript pause; // PauseMenu
    public GameObject failPopUp;

    //private List<string> colors = new List<string> { "Red", "Blue", "Green", "Yellow" };
    private List<string> directions = new List<string> { "Up", "Down", "Left", "Right" };
    private Dictionary<string, KeyCode> DirectionKeyMap = new Dictionary<string, KeyCode>
    {
        { "Up", KeyCode.UpArrow },
        { "Down", KeyCode.DownArrow },
        { "Left", KeyCode.LeftArrow },
        { "Right", KeyCode.RightArrow }
    };
    /*private Dictionary<string, KeyCode> ColorKeyMap = new Dictionary<string, KeyCode>
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
    };*/

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

    public AudioSource BackgroundMusicSource;

    public AudioSource CountdownAudioSource;
    public AudioClip CountdownBeepClip;
    //public AudioClip CountdownFinalClip; // For "DANCE!" after the numbers.

    private void Awake()
    {
        InputActions = new PlayerInputActions();

        /*InputActions.DancingSeedPlayer.MoveUp.performed += ctx => OnMove("Yellow");
        InputActions.DancingSeedPlayer.MoveDown.performed += ctx => OnMove("Green");
        InputActions.DancingSeedPlayer.MoveLeft.performed += ctx => OnMove("Blue");
        InputActions.DancingSeedPlayer.MoveRight.performed += ctx => OnMove("Red");*/

        InputActions.DancingSeedPlayer.MoveUp.performed += ctx => OnMove("Up");
        InputActions.DancingSeedPlayer.MoveDown.performed += ctx => OnMove("Down");
        InputActions.DancingSeedPlayer.MoveLeft.performed += ctx => OnMove("Left");
        InputActions.DancingSeedPlayer.MoveRight.performed += ctx => OnMove("Right");

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
        TotalRounds = Random.Range(3, 6); // Rounds 3 - 5 are included since Random.Range with int maxExlusive is exclusive. (int minInclusive, int maxExclusive).
        switch (TotalRounds)
        {
            case 3:
                TimeLimit = 20f;
                break;
            case 4:
                TimeLimit = 25f;
                break;
            case 5:
                TimeLimit = 30f;
                break;
        }

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

            if (CountdownAudioSource != null)
            {
                if (number == "DANCE!")
                {
                    // Play DANCE! sound if there is one.
                    /*if (CountdownFinalClip != null)
                    {
                        CountdownAudioSource.PlayOneShot(CountdownFinalClip);
                    }*/
                }
                else
                {
                    if (CountdownBeepClip != null && !CountdownAudioSource.isPlaying)
                    {
                        CountdownAudioSource.PlayOneShot(CountdownBeepClip);
                    }
                }
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
            if(!pause.GameIsPaused)
                    pause.Paused();
                else
                    pause.Resume();
        }

        if (Input.anyKeyDown)
        {
            foreach (var direction in directions)
            {
                if (Input.GetKeyDown(DirectionKeyMap[direction]))
                {
                    CheckInput(direction);
                    ChangeSeedPose(direction);
                    break;
                }
            }
        }
    }

    private void OnMove(string DirectionName)
    {
        Debug.Log("OnMove called with DirectionName " + DirectionName);
        if (!IsInputEnabled)
            return;

        //ChangeSeedPose(ColorName);

        CheckInput(DirectionName);
    }

    private void ChangeSeedPose(string DirectionName)
    {
        Debug.Log("ChangeSeedPose called with DirectionName" + DirectionName);
        Sprite NewSprite = IdleSprite;

        switch(DirectionName)
        {
            case "Right":
                NewSprite = RightPoseSprite;
                break;
            case "Left":
                NewSprite = LeftPoseSprite;
                break;
            case "Down":
                NewSprite = DownPoseSprite;
                break;
            case "Up":
                float chance = Random.Range(0f, 1f);
                if (chance < 0.3f)
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
                Debug.LogWarning("Unknown DirectionName: " + DirectionName);
                break;
        }

        PlayerRenderer.sprite = NewSprite;
        Debug.Log("Changed sprite to " + NewSprite.name);

        CancelInvoke("ResetSeedPose");
        if (CurrentRound >= TotalRounds)
        {
            Invoke("ResetSeedPose", 0.5f);
        }
        else
        {
            Invoke("ResetSeedPose", 1.5f);
        }
    }

    private void ResetSeedPose()
    {
        if (CurrentRound >= TotalRounds)
        {
            PlayerRenderer.sprite = TransformationSprite;
        }
        else
        {
            PlayerRenderer.sprite = IdleSprite;
        }
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
            string direction = directions[Random.Range(0, directions.Count)];
            CurrentSequence.Add(direction);
        }

        CurrentInputIndex = 0;

        // Display the first prompt here.
        DisplayCurrentPrompt();

        //TimerText.color = Color.white;
        UpdateTimerDisplay();

        if (BackgroundMusicSource != null && !BackgroundMusicSource.isPlaying)
        {
            BackgroundMusicSource.Play();
        }


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

        // Instantiate the current prompt UI using an arrow as the prompt.
        string direction = CurrentSequence[CurrentInputIndex];
        GameObject prompt = Instantiate(ArrowPromptPrefab, KeyPromptPanel);
        RectTransform PromptRect = prompt.GetComponent<RectTransform>();

        float RotationAngle = 0f;

        switch (direction)
        {
            case "Up":
                RotationAngle = 0f;
                break;
            case "Right":
                RotationAngle = 270f;
                break;
            case "Down":
                RotationAngle = 180f;
                break;
            case "Left":
                RotationAngle = 90f;
                break;
        }

        PromptRect.localRotation = Quaternion.Euler(0f, 0f, RotationAngle);

        // Instantiate the current prompt UI.
        /*string ColorName = CurrentSequence[CurrentInputIndex];
        GameObject prompt = Instantiate(KeyPromptPrefab, KeyPromptPanel);
        Image PromptImage = prompt.GetComponent<Image>();
        if (PromptImage != null)
        {
            PromptImage.color = ColorMap[ColorName];
        }*/
    }

    void CheckInput(string InputDirection)
    {
        if(InputDirection == CurrentSequence[CurrentInputIndex])
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

        if (BackgroundMusicSource != null && BackgroundMusicSource.isPlaying)
        {
            BackgroundMusicSource.Stop();
        }

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

        //PlayerRenderer.color = Color.white;

        IsInputEnabled = false;

        IsTimerRunning = false;

        if (RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(true);
            RoundCompleteText.text = "Game Complete!";
        }

        Invoke("ProceedToNextMinigame", 3f);
    }

    void ProceedToNextMinigame()
    {
        if (RoundCompleteText != null)
        {
            RoundCompleteText.gameObject.SetActive(false);
        }

        Debug.Log("Proceeding to the next minigame.");

        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int NextSceneIndex = CurrentSceneIndex + 1;

        if (NextSceneIndex < SceneManager. sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(NextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes. Maybe pause the game and exit?");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RubThePotionGame : MonoBehaviour
{
    // Potion animation here.
    public Animator PotionAnimator;
    private int PotionFillLevel = 0;
    private int MaxPotionFillLevel = 18;

    // Hand movement here.
    public Transform HandTransform;
    private Vector3 HandStartPosition;
    public float HandMoveRadius = 0.2f;
    public float HandMoveDuration = 0.5f;

    // Input Sequence
    private string[] RequiredSequence = new string[]
    {
        "Left", "Left", "Left",
        "Right", "Right", "Right"
    };
    private int SequenceIndex = 0;

    private bool IsSequenceCompleted = false;

    // Timer Variables here.
    public float TimeLimit = 30f;
    private float TimeRemaining;
    public TextMeshProUGUI TimerText;
    private bool IsTimerRunning = false;

    private bool IsBlinking = false;

    // Attempts variables here.
    public int MaxIncorrectAttempts = 2;
    private int IncorrectAttempts = 0;
    public TextMeshProUGUI AttemptsText;

    // UI Panels here.
    public GameObject GameCompletePanel;
    public GameObject GameOverPanel;

    // Audio clips here.
    public AudioClip WinSound;

    // The audio source.
    private AudioSource AudioSource;

    // The new input system.
    private PlayerInputActions PlayerInputActions;

    // Sprites
    public SpriteRenderer BottleSpriteRenderer;
    public Sprite CompletedPotionSprite;
    private Sprite OriginalBottleSprite;

    // References to GameObjects.
    public GameObject BottleGameObject;
    public GameObject OtherBottleGameObject;
    public GameObject CompletedPotionGameObject;
    public GameObject Sparkles1;
    public GameObject Sparkles2;

    private Coroutine SparkleCoroutine;

    private void Awake()
    {
        PlayerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        PlayerInputActions.RubThePotionPlayer.Enable();

        // Subscribe to the input events here.
        PlayerInputActions.RubThePotionPlayer.RubLeft.performed += OnRubLeft;
        PlayerInputActions.RubThePotionPlayer.RubRight.performed += OnRubRight;
    }

    private void OnDisable()
    {
        // Unsubscribe from the input events here.
        PlayerInputActions.RubThePotionPlayer.RubLeft.performed -= OnRubLeft;
        PlayerInputActions.RubThePotionPlayer.RubRight.performed -= OnRubRight;

        PlayerInputActions.RubThePotionPlayer.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeRemaining = TimeLimit;
        IsTimerRunning = true;

        if (TimerText != null)
        {
            UpdateTimerDisplay();
            TimerText.color = Color.white;
        }
        if (AttemptsText != null)
        {
            AttemptsText.text = "Attempts Left: " + (MaxIncorrectAttempts - IncorrectAttempts);
        }
        if (GameCompletePanel != null)
            GameCompletePanel.SetActive(false);
        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);

        if (HandTransform != null)
        {
            HandStartPosition = HandTransform.position;
        }

        AudioSource = GetComponent<AudioSource>();

        UpdatePotionAnimation();

        if (BottleSpriteRenderer != null)
        {
            OriginalBottleSprite = BottleSpriteRenderer.sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSequenceCompleted)
            return;

        if (IsTimerRunning)
        {
            TimeRemaining -= Time.deltaTime;

            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                IsTimerRunning = false;
                UpdateTimerDisplay();
                GameOver("Time's up!");
                return;
            }

            UpdateTimerDisplay();

            if (TimeRemaining <= 5f)
            {
                BlinkTimer();
            }
        }
    }

    private void OnRubLeft(InputAction.CallbackContext context)
    {
        if (!IsSequenceCompleted)
        {
            CheckInput("Left");
            StartCoroutine(HandRubCoroutine(-1));
        }
    }
    private void OnRubRight(InputAction.CallbackContext context)
    {
        if (!IsSequenceCompleted)
        {
            CheckInput("Right");
            StartCoroutine(HandRubCoroutine(1));
        }
    }

    void CheckInput(string InputDirection)
    {
        if (InputDirection == RequiredSequence[SequenceIndex])
        {
            SequenceIndex++;

            // Increment potion fill level by 3 per correct input.
            PotionFillLevel = Mathf.Min(PotionFillLevel + 3, MaxPotionFillLevel);

            //PlaySound(CorrectInputSound);

            UpdatePotionAnimation();

            // Check if the entire sequence has been completed.
            if (SequenceIndex >= RequiredSequence.Length)
            {
                SequenceCompleted();
            }
        }
        else
        {
            IncorrectAttempts++;
            if (AttemptsText != null)
            {
                AttemptsText.text = "Attempts Left: " + (MaxIncorrectAttempts - IncorrectAttempts);
            }

            if (IncorrectAttempts >= MaxIncorrectAttempts)
            {
                GameOver("Too many incorrect attempts.");
                return;
            }

            //PlaySound(IncorrectInputSound);
            ResetSequence();
        }
    }

    void ResetSequence()
    {
        SequenceIndex = 0;
        PotionFillLevel = 0;
        UpdatePotionAnimation();
    }

    void UpdatePotionAnimation()
    {
        if (PotionAnimator != null)
        {
            // Normalized (So, between 0 and 1).
            float NormalizedTime = (float)PotionFillLevel / MaxPotionFillLevel;
            NormalizedTime = Mathf.Clamp01(NormalizedTime);

            PotionAnimator.Play("PotionFillAnim", 0, NormalizedTime);
            PotionAnimator.speed = 0; // Pause the animator to keep the animation from continuously playing.
        }
    }

    IEnumerator HandRubCoroutine(int direction)
    {
        float ElapsedTime = 0f;

        while (ElapsedTime < HandMoveDuration)
        {
            ElapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 360f, ElapsedTime / HandMoveDuration) * direction;
            float radian = angle * Mathf.Deg2Rad;

            float x = HandStartPosition.x + Mathf.Cos(radian) * HandMoveRadius;
            float y = HandStartPosition.y + Mathf.Sin(radian) * HandMoveRadius;

            HandTransform.position = new Vector3(x, y, HandTransform.position.z);

            yield return null;
        }

        HandTransform.position = HandStartPosition;
    }

    void SequenceCompleted()
    {
        IsSequenceCompleted = true;
        IsTimerRunning = false;

        PotionFillLevel = MaxPotionFillLevel;
        UpdatePotionAnimation();

        if (BottleGameObject != null)
        {
            BottleGameObject.SetActive(false);
        }
        if (OtherBottleGameObject != null)
        {
            OtherBottleGameObject.SetActive(false);
        }
        if (CompletedPotionGameObject != null)
        {
            CompletedPotionGameObject.SetActive(true);
        }

        SparkleCoroutine = StartCoroutine(SparkleBlinkCoroutine());

        StartCoroutine(LoadNextLevelAfterDelay(5f));

        PlaySound(WinSound);

        Debug.Log("Potion is ready! You win!");

        if (GameCompletePanel != null)
            GameCompletePanel.SetActive(true);
    }

    void GameOver(string reason)
    {
        IsSequenceCompleted = true;
        IsTimerRunning = false;

        Debug.Log("Game over! " + reason);

        //PlaySound(LoseSound);

        if (GameOverPanel != null)
            GameOverPanel.SetActive(true);
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

    IEnumerator SparkleBlinkCoroutine()
    {
        // Activate the sparkles initially.
        if (Sparkles1 != null) Sparkles1.SetActive(true);
        if (Sparkles2 != null) Sparkles2.SetActive(false);

        bool toggle = false;
        float BlinkInterval = 0.5f;

        while (true)
        {
            toggle = !toggle;
            if (Sparkles1 != null) Sparkles1.SetActive(toggle);
            if (Sparkles2 != null) Sparkles2.SetActive(!toggle);

            yield return new WaitForSeconds(BlinkInterval);
        }
    }

    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        int NextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (NextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(NextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes to load!");
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && AudioSource != null)
        {
            AudioSource.PlayOneShot(clip);
        }
    }

    public void RetryGame()
    {
        // Reset the minigame variables.
        IsSequenceCompleted = false;
        SequenceIndex = 0;
        IncorrectAttempts = 0;
        TimeRemaining = TimeLimit;
        IsBlinking = false;
        IsTimerRunning = true;
        PotionFillLevel = 0;

        // Reset the UI elements.
        if (AttemptsText != null)
        {
            AttemptsText.text = "Attempts Left: " + (MaxIncorrectAttempts - IncorrectAttempts);
        }
        if (TimerText != null)
        {
            UpdateTimerDisplay();
            TimerText.color = Color.white;
        }

        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);
        if (GameCompletePanel != null)
            GameCompletePanel.SetActive(false);

        if (BottleGameObject != null)
        {
            BottleGameObject.SetActive(true);
        }
        if (CompletedPotionGameObject != null)
        {
            CompletedPotionGameObject.SetActive(false);
        }

        if (SparkleCoroutine != null)
        {
            StopCoroutine(SparkleCoroutine);
            SparkleCoroutine = null;
        }
        if (Sparkles1 != null) Sparkles1.SetActive(false);
        if (Sparkles2 != null) Sparkles2.SetActive(false);
    }
}

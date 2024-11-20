using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RubThePotionGame : MonoBehaviour
{
    public SpriteRenderer PotionSpriteRenderer;

    public Color NewPotionColor = Color.green;
    private Color OriginalPotionColor;
    private KeyCode[] RequiredSequence = new KeyCode[]
    {
        KeyCode.LeftArrow, KeyCode.LeftArrow, KeyCode.LeftArrow,
        KeyCode.RightArrow, KeyCode.RightArrow, KeyCode.RightArrow
    };

    private int SequenceIndex = 0;

    private bool IsSequenceCompleted = false;

    public float TimeLimit = 20f;
    private float TimeRemaining;
    public TextMeshProUGUI TimerText;
    private bool IsTimerRunning = false;

    public int MaxIncorrectAttempts = 3;
    private int IncorrectAttempts = 0;
    public TextMeshProUGUI AttemptsText;

    public GameObject GameCompletePanel;
    public GameObject GameOverPanel;

    //public AudioClip CorrectInputSound;
    //public AudioClip IncorrectInoutSound;
    //public AudioClip WinSound;
    //public AudioClip LoseSound;
    //public AudioClip LowTimeSound;
    //private bool HasPlayedLowTimeSound = false;

    private bool IsBlinking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        TimeRemaining = TimeLimit;
        OriginalPotionColor = PotionSpriteRenderer.color;

        if (TimerText != null)
        {
            UpdateTimerDisplay();
            TimerText.color = Color.white;
        }
        if(AttemptsText != null)
        {
            AttemptsText.text = "Attempts left: " + (MaxIncorrectAttempts - IncorrectAttempts);
        }
        if (GameCompletePanel != null)
        {
            GameCompletePanel.SetActive(false);
        }
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(false);
        }

        IsTimerRunning = true;
    }*/

    // Update is called once per frame
    /*void Update()
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

                UpdateTimerDisplay();

                if (TimeRemaining <= 5f)
                {
                    if (!HasPlayedLowTimeSound)
                    {
                        PlaySound(LowTimeSound);
                        HasPlayedLowTimeSound = true;
                    }

                    BlinkTimer();
                }
            }
        }
    }*/
}

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HeatThePotionGameManagerEndless : MonoBehaviour
{
    public ProgressBarControllerEndless ProgressBarController;
    public HeatKnobController HeatKnobController;
    public DirectionArrowController DirectionArrowController;
    public int RequiredSuccesses = 3;
    public TextMeshProUGUI FeedbackText;

    private AudioSource AudioSource;

    public SpriteRenderer PotionSpriteRenderer;
    private Color OriginalPotionColor;

    private int CurrentSuccesses = 0;
    private bool InputAllowed = false;
    private string RequiredDirection;
    private bool PlayerDidCorrectAction = false;

    public PauseScript pause; // PauseMenu
    public GameObject failPopUp;
    public GameObject instruct; // hi
    public GameObject successPopUp;

    public GameObject bar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Intro();

        if (PotionSpriteRenderer != null)
        {
            OriginalPotionColor = PotionSpriteRenderer.color;
        }

        HeatKnobController.OnKnobRotated += HandleKnobRotation;
        StartNewSequence();

        AudioSource = GetComponent<AudioSource>();
    }
    void Update()
    {

        // Hey Antonio this was the only way I could get the pause menu to work pls don't delete -Asha
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!pause.GameIsPaused)
                pause.Paused();
            else
                pause.Resume();
        }
    

    }

    private void OnDestroy()
    {
        HeatKnobController.OnKnobRotated -= HandleKnobRotation;
    }

    public void SafeZoneStarted()
    {
        InputAllowed = true;
        PlayerDidCorrectAction = false;
    }

    public void SafeZoneEnded()
    {
        InputAllowed = false;

        if (PlayerDidCorrectAction)
        {
            SuccessfulAction();
        }
        else
        {
            FailedAction();
        }
    }

    public void Intro() // coding for Instructions popup
    {
        instruct.SetActive(true);
        pause.GameIsPaused = true;
       // IsTimerRunning = false;
        Time.timeScale = 0;
        HideFeedback();
        bar.SetActive(false);
    }

    private void HandleKnobRotation()
    {
        if (!InputAllowed)
        {
            float KnobRotation = HeatKnobController.GetCurrentZRotation();
            if (KnobRotation != 0f)
            {
                FailedAction();
            }
            return;
        }

        if (ProgressBarController.IsWithinSafeZone())
        {
            float KnobRotation = HeatKnobController.GetCurrentZRotation();

            //Debug.Log($"Knob rotated within safe zone. KnobRotation: {KnobRotation}, RequiredDirection: {RequiredDirection}");
            
            if (RequiredDirection == "Left" && KnobRotation >= 65f && KnobRotation <= 85f)
            {
                PlayerDidCorrectAction = true;
            }
            else if (RequiredDirection == "Right" && KnobRotation <= -65f && KnobRotation >= -85f)
            {
                PlayerDidCorrectAction = true;
            }
            else
            {
                Debug.Log("The knob has been rotated to an incorrect position within the safe zone.");
            }
        }
        else
        {
            Debug.Log("The knob has been rotated outside of the safe zone.");
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    private void ResetPotionColor()
    {
        if (PotionSpriteRenderer != null)
        {
            PotionSpriteRenderer.color = OriginalPotionColor;
        }
    }

    private IEnumerator TransitionPotionColor(Color TargetColor, float duration)
    {
        if (PotionSpriteRenderer != null)
        {
            Color InitialColor = PotionSpriteRenderer.color;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                PotionSpriteRenderer.color = Color.Lerp(InitialColor, TargetColor, time / duration);
                yield return null;
            }
            PotionSpriteRenderer.color = TargetColor;
        }
    }

    private void SuccessfulAction()
    {
        HeatKnobController.ResetKnob();

        Debug.Log("Successful " + RequiredDirection + " rotation!");
        ShowFeedback("Success!");

        if (PotionSpriteRenderer != null)
        {
            Color RandomColor = GetRandomColor();
            StartCoroutine(TransitionPotionColor(RandomColor, 1f));
        }

        CurrentSuccesses++;

        if (CurrentSuccesses >= RequiredSuccesses)
        {
            // The minigame is now complete.
            Debug.Log("The potion is ready!");
            successPopUp.SetActive(true);

            if (AudioSource != null)
            {
                AudioSource.Play();
            }

            Invoke("ProceedToNextMinigame", 3f);
        }
    }

    private void FailedAction()
    {
        HeatKnobController.ResetKnob();

        if (PotionSpriteRenderer != null)
        {
            Color FailColor = Color.black;
            StartCoroutine(TransitionPotionColor(FailColor, 1f));
        }

        Debug.Log("Failed " + RequiredDirection + " rotation!");
        ShowFeedback("Fail!");
        failPopUp.SetActive(true);
    }

    public void StartNewSequence()
    {
        ResetPotionColor();
        HeatKnobController.ResetKnob();

        FeedbackText.text = "";
        FeedbackText.gameObject.SetActive(false);

        SetRequiredRotationDirection();

        Debug.Log("Press the " + GetRequiredDirectionKey() + " key!");

        ProgressBarController.SetSafeZoneDirectionText(GetRequiredDirectionKey());

        ProgressBarController.PositionSafeZone();
        ProgressBarController.ResetProgress();
    }

    public void SetRequiredRotationDirection()
    {
        // Randomly select from "Left" or "Right" here.
        RequiredDirection = Random.value > 0.5f ? "Left" : "Right";

        if (RequiredDirection == "Left")
        {
            DirectionArrowController.PointLeft();
        }
        else
        {
            DirectionArrowController.PointRight();
        }
    }

    public string GetRequiredDirectionKey()
    {
        return RequiredDirection == "Left" ? "Left Arrow" : "Right Arrow";
    }

    public void ShowFeedback(string message)
    {
        FeedbackText.text = message;
        FeedbackText.gameObject.SetActive(true);
        Invoke("HideFeedback", 3f);
    }

    private void HideFeedback()
    {
        FeedbackText.gameObject.SetActive(false);
    }

    void ProceedToNextMinigame()
    {
        Debug.Log("Proceeding to a random minigame.");

        int MinSceneIndex = 7;
        int MaxSceneIndex = 11;
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check to make sure that MaxSceneIndex doesn't exceed the number of scenes in build settings.
        MaxSceneIndex = Mathf.Min(MaxSceneIndex, SceneManager.sceneCountInBuildSettings - 1);

        // Create a list of possible scene indices excluding the current scene to avoid repeating the same scene for next minigame.
        List<int> PossibleSceneIndices = new List<int>();
        for (int i = MinSceneIndex; i <= MaxSceneIndex; i++)
        {
            if (i != CurrentSceneIndex)
            {
                PossibleSceneIndices.Add(i);
            }
        }

        if (PossibleSceneIndices.Count > 0)
        {
            // Select a random index from the list of possible scene indices.
            int RandomIndex = Random.Range(0, PossibleSceneIndices.Count);
            int RandomSceneIndex = PossibleSceneIndices[RandomIndex];

            // Load the randomly selected scene other than the current scene.
            SceneManager.LoadScene(RandomSceneIndex);
        }
        else
        {
            Debug.LogWarning("No other scenes available to load, try adding scenes to the build settings.");
        }
    }
}

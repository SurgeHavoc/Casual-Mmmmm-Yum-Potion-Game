using UnityEngine;
using TMPro;

public class HeatThePotionGameManager : MonoBehaviour
{
    public ProgressBarController ProgressBarController;
    public HeatKnobController HeatKnobController;
    public int RequiredSuccesses = 3;
    public TextMeshProUGUI FeedbackText;

    private int CurrentSuccesses = 0;
    private bool InputAllowed = false;
    private string RequiredDirection;
    private bool PlayerDidCorrectAction = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HeatKnobController.OnKnobRotated += HandleKnobRotation;
        StartNewSequence();
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

    private void HandleKnobRotation()
    {
        if (!InputAllowed)
            return;

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

    private void SuccessfulAction()
    {
        HeatKnobController.ResetKnob();

        Debug.Log("Successful " + RequiredDirection + " rotation!");
        ShowFeedback("Success!");

        CurrentSuccesses++;

        if (CurrentSuccesses >= RequiredSuccesses)
        {
            // The minigame is now complete.
            Debug.Log("The potion is ready!");
        }
    }

    private void FailedAction()
    {
        HeatKnobController.ResetKnob();

        Debug.Log("Failed " + RequiredDirection + " rotation!");
        ShowFeedback("Fail!");
    }

    public void StartNewSequence()
    {
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
    }

    public string GetRequiredDirectionKey()
    {
        return RequiredDirection == "Left" ? "Left Arrow" : "Right Arrow";
    }

    private void ShowFeedback(string message)
    {
        FeedbackText.text = message;
        FeedbackText.gameObject.SetActive(true);

        Invoke("HideFeedback", 3f);
    }

    private void HideFeedback()
    {
        FeedbackText.gameObject.SetActive(false);
    }
}

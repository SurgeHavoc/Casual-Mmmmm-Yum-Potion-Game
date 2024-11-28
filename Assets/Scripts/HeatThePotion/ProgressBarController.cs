using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarController : MonoBehaviour
{
    public Image ProgressBarFill;
    public Image SafeZone;
    public HeatThePotionGameManager GameManager;
    public float ProgressSpeed = 0.2f;
    public TextMeshProUGUI SafeZoneDirectionText;

    private bool IsIncreasing = true;
    private bool InSafeZone = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // An empty bar upon game start.
        ProgressBarFill.fillAmount = 0;
        PositionSafeZone();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProgressBar();
    }

    void UpdateProgressBar()
    {
        if (IsIncreasing)
            ProgressBarFill.fillAmount += ProgressSpeed * Time.deltaTime;

        if (ProgressBarFill.fillAmount >= 1)
        {
            ProgressBarFill.fillAmount = 1;
            ResetProgress();
            GameManager.StartNewSequence();
        }

        // Check here if within the safe zone.
        bool WasInSafeZone = InSafeZone;
        InSafeZone = IsInSafeZone();

        if (!WasInSafeZone && InSafeZone)
        {
            //Debug.Log("Entered Safe Zone at progress: " + ProgressBarFill.fillAmount);
            GameManager.SafeZoneStarted();
        }
        else if (WasInSafeZone && !InSafeZone)
        {
            //Debug.Log("Exited Safe Zone at progress: " + ProgressBarFill.fillAmount);
            GameManager.SafeZoneEnded();
        }
    }

    bool IsInSafeZone()
    {
        float ProgressBarWidth = ProgressBarFill.rectTransform.rect.width;

        float CurrentProgressPosition = ProgressBarWidth * ProgressBarFill.fillAmount;

        float SafeZoneStartAnchor = SafeZone.rectTransform.anchorMin.x;
        float SafeZoneEndAnchor = SafeZone.rectTransform.anchorMax.x;
        float SafeZoneWidth = ProgressBarWidth * (SafeZoneEndAnchor - SafeZoneStartAnchor);
        float SafeZonePosition = ProgressBarWidth * SafeZoneStartAnchor;

        bool IsInSafeZone = CurrentProgressPosition >= SafeZonePosition && CurrentProgressPosition <= (SafeZonePosition + SafeZoneWidth);

        return IsInSafeZone;
    }

    public bool IsWithinSafeZone()
    {
        return InSafeZone;
    }

    public void ResetProgress()
    {
        ProgressBarFill.fillAmount = 0;
        PositionSafeZone();
        IsIncreasing = true;
    }

    public void PositionSafeZone()
    {
        // Here is where the safe zone is randomly positioned.
        float RandomPosition = Random.Range(0.1f, 0.7f);
        float SafeZoneWidth = 0.2f;

        float VerticalSize = 0.5f;
        float VerticalOffset = (1 - VerticalSize) / 2;

        SafeZone.rectTransform.anchorMin = new Vector2(RandomPosition, VerticalOffset);
        SafeZone.rectTransform.anchorMax = new Vector2(RandomPosition + SafeZoneWidth, 1 - VerticalOffset);
        SafeZone.rectTransform.offsetMin = Vector2.zero;
        SafeZone.rectTransform.offsetMax = Vector2.zero;
    }

    public void SetSafeZoneDirectionText(string direction)
    {
        if (SafeZoneDirectionText != null)
        {
            SafeZoneDirectionText.text = direction;
        }
    }
}

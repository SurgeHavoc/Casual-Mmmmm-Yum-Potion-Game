using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarControllerEndless : MonoBehaviour
{
    public SpriteRenderer ProgressBarFill;
    public SpriteRenderer SafeZone;
    public HeatThePotionGameManagerEndless GameManager;
    public float ProgressSpeed = 0.2f;
    public TextMeshProUGUI SafeZoneDirectionText;

    public bool IsIncreasing = true;
    private bool InSafeZone = false;

    private float progress = 0f;
    private float InitialScaleX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialScaleX = ProgressBarFill.transform.localScale.x;

        progress = 0f;
        UpdateProgressBarVisual();
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
        {
            progress += ProgressSpeed * Time.deltaTime;
            progress = Mathf.Clamp01(progress);

            UpdateProgressBarVisual();

            if (progress >= 1f)
            {
                ResetProgress();
                GameManager.StartNewSequence();
            }
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

    void UpdateProgressBarVisual()
    {
        // Adjust the scale of the progress bar based on the current progress position.
        float NewScaleX = InitialScaleX * progress;
        ProgressBarFill.transform.localScale = new Vector3(NewScaleX, ProgressBarFill.transform.localScale.y, ProgressBarFill.transform.localScale.z);
    }

    bool IsInSafeZone()
    {
        float ProgressPosition = ProgressBarFill.transform.position.x  + ProgressBarFill.bounds.size.x;
        float SafeZoneStart = SafeZone.bounds.min.x;
        float SafeZoneEnd = SafeZone.bounds.max.x;

        return ProgressPosition >= SafeZoneStart && ProgressPosition <= SafeZoneEnd;
    }

    public bool IsWithinSafeZone()
    {
        return InSafeZone;
    }

    public void ResetProgress()
    {
        progress = 0f;
        UpdateProgressBarVisual();
        PositionSafeZone();
        IsIncreasing = true;
    }

    public void PositionSafeZone()
    {
        // Get the progress bar background sprite renderer to get the bounds of the background.
        SpriteRenderer ProgressBarBackground = ProgressBarFill.transform.parent.GetComponent<SpriteRenderer>();

        // Get the left and right edges of the progress bar background.
        float LeftEdge = ProgressBarBackground.bounds.min.x;
        float RightEdge = ProgressBarBackground.bounds.max.x;

        float SafeZoneWidth = SafeZone.bounds.size.x;

        // Add margins to define the region within the progress bar background.
        float TotalWidth = RightEdge - LeftEdge;
        float MarginPercentage = 0.1f;
        float LeftMargin = TotalWidth * MarginPercentage;
        float RightMargin = TotalWidth * MarginPercentage;

        float MinX = LeftEdge + LeftMargin + SafeZoneWidth / 2;
        float MaxX = RightEdge - RightMargin - SafeZoneWidth / 2;

        if (MinX >= MaxX)
        {
            Debug.LogWarning("Safe zone margins are too large, adjust the margins accordingly.");
            MinX = MaxX = (LeftEdge + RightEdge) / 2;
        }

        float RandomX = Random.Range(MinX, MaxX);

        Vector3 NewPosition = new Vector3(RandomX, SafeZone.transform.position.y, SafeZone.transform.position.z);
        SafeZone.transform.position = NewPosition;
    }

    public void SetSafeZoneDirectionText(string direction)
    {
        if (SafeZoneDirectionText != null)
        {
            SafeZoneDirectionText.text = direction;
        }
    }
}

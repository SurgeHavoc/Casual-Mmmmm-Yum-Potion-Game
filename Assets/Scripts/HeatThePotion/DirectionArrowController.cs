using UnityEngine;

public class DirectionArrowController : MonoBehaviour
{
    private RectTransform ArrowRectTransform;

    private void Awake()
    {
        ArrowRectTransform = GetComponent<RectTransform>();
    }

    public void PointLeft()
    {
        ArrowRectTransform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public void PointRight()
    {
        ArrowRectTransform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    public void HideArrow()
    {
        gameObject.SetActive(false);
    }

    public void ShowArrow()
    {
        gameObject.SetActive(true);
    }
}

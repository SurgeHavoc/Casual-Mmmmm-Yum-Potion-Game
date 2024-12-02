using UnityEngine;

public class DirectionArrowController : MonoBehaviour
{
    private Transform ArrowTransform;

    private void Awake()
    {
        ArrowTransform = GetComponent<Transform>();
    }

    public void PointLeft()
    {
        ArrowTransform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public void PointRight()
    {
        ArrowTransform.localRotation = Quaternion.Euler(0, 0, -90);
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

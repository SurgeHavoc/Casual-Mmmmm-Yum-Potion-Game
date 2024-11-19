using UnityEngine;

public class Pestle : MonoBehaviour
{
    private bool IsDragging = false;
    private Camera MainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        IsDragging = true;
    }

    private void OnMouseUp()
    {
        IsDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDragging)
        {
            Vector3 MousePosition = Input.mousePosition;
            MousePosition.z = Mathf.Abs(MainCamera.transform.position.z);
            transform.position = MainCamera.ScreenToWorldPoint(MousePosition);
        }
        
    }
}

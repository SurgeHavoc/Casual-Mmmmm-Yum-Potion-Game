using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool IsDragging = false;
    private Vector3 offset;
    private Camera MainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCamera = Camera.main;
    }

    private void OnMouseDown()
    {
        IsDragging = true;

        // Calculate the offset between the mouse position and the sprite's position.
        offset = transform.position - GetMouseWorldPosition();
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
            // Update the position while dragging the sprite.
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    // A helper method to get the mouse position in world coordinates.
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 MousePosition = Input.mousePosition;
        MousePosition.z = Mathf.Abs(MainCamera.transform.position.z);
        return MainCamera.ScreenToWorldPoint(MousePosition);
    }
}

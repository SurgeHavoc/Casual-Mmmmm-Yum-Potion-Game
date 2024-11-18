using UnityEngine;

public class MortarIngredient : MonoBehaviour
{
    public string IngredientName;
    [HideInInspector] public Vector3 OriginalPosition;

    private bool IsDragging = false;
    private Camera MainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCamera = Camera.main;
        OriginalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        IsDragging = true;
    }

    private void OnMouseUp()
    {
        IsDragging = false;

        // If mouse up, ingredient resets position.
        transform.position = OriginalPosition;
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

    private bool IsInsideMortar = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mortar"))
        {
            IsInsideMortar = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Mortar"))
        {
            IsInsideMortar = false;
        }
    }
}

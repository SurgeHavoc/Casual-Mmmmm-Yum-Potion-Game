using UnityEngine;

public class MortarIngredientEndless : MonoBehaviour
{
    public string IngredientName;
    [HideInInspector] public Vector3 OriginalPosition;
    private Camera MainCamera;

    private bool IsDragging = false;
    private bool IsClone = false;
    private GameObject OriginalIngredient;

    private MortarEndless mortar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainCamera = Camera.main;
        OriginalPosition = transform.position;

        mortar = FindFirstObjectByType<MortarEndless>();
    }

    private void OnMouseDown()
    {
        if (mortar.IsIngredientDraggable)
        {
            if (!IsClone)
            {
                GameObject clone = Instantiate(gameObject, OriginalPosition, Quaternion.identity);
                clone.GetComponent<MortarIngredientEndless>().IsClone = false;

                IsClone = true;
                OriginalIngredient = clone;
            }
            IsDragging = true;
        }
    }

    private void OnMouseUp()
    {
        IsDragging = false;

        if (mortar.IsIngredientDraggable)
        {
            Destroy(gameObject);
        }

        // If mouse up, ingredient resets position.
        //transform.position = OriginalPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDragging && mortar.IsIngredientDraggable)
        {
            Vector3 MousePosition = Input.mousePosition;
            MousePosition.z = Mathf.Abs(MainCamera.transform.position.z);
            transform.position = MainCamera.ScreenToWorldPoint(MousePosition);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mortar"))
        {
            IsDragging = false;

            //mortar.IngredientAttempted(this);

            Destroy(gameObject);
        }
    }
}
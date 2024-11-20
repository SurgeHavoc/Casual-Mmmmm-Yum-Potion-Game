using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClawHandController : MonoBehaviour
{
    public enum HandState
    {
        MovingHorizontally,
        MovingDown,
        PickingUp,
        MovingToDropOff,
        Resetting
    }
    public HandState CurrentState = HandState.MovingHorizontally;

    public float MoveSpeed = 500f; // Speed of hand horizontally in pixels per second.
    public float LeftBoundary = -910f;
    public float RightBoundary = 910f;
    public float DownSpeed = 300f; // Speed of hand moving down in pixels per second.
    public float DownBoundary = -500f;
    public float ResetSpeed = 500f;
    public float MoveToDropOffSpeed = 500f;

    private RectTransform RectTransform;
    private Vector2 InitialPosition;
    private bool MovingRight = true;
    private List<RectTransform> IngredientRects;
    private GameObject CarriedIngredient = null;

    public RectTransform DropOffPoint;
    public Button GrabButton;
    private bool IsButtonReady = true;

    private PlayerInputActions InputActions;

    public Sprite OpenHandSprite;
    public Sprite PinchedHandSprite;

    private Image HandImage;

    private void Awake()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.ClawHandControllerInstance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HandImage = GetComponent<Image>();
        RectTransform = GetComponent<RectTransform>();
        InitialPosition = RectTransform.anchoredPosition;

        // Get list of ingredient RectTransforms here.
        IngredientRects = new List<RectTransform>();
        GameObject[] IngredientObjects = GameObject.FindGameObjectsWithTag("Ingredient");
        foreach (GameObject ingredient in IngredientObjects)
        {
            IngredientRects.Add(ingredient.GetComponent<RectTransform>());
        }

        GrabButton.onClick.AddListener(OnGrabButtonClicked);
        SetButtonState(true);

        InputActions = new PlayerInputActions();
        InputActions.PickTheIngredientsPlayer.Enable();

        InputActions.PickTheIngredientsPlayer.Grab.performed += OnGrabAction;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        switch (CurrentState)
        {
            case HandState.MovingHorizontally:
                MoveClawHandHorizontally();
                break;
            case HandState.MovingDown:
                MoveClawHandDown();
                break;
            case HandState.PickingUp:
                // A brief state, before proceeding to move to drop-off point.
                CurrentState = HandState.MovingToDropOff;
                break;
            case HandState.MovingToDropOff:
                MoveToDropOff();
                break;
            case HandState.Resetting:
                ResetClawHandPosition();
                break;
        }
    }

    void MoveClawHandHorizontally()
    {
        float step = MoveSpeed * Time.deltaTime;
        if (MovingRight)
        {
            RectTransform.anchoredPosition += new Vector2(step, 0);
            if (RectTransform.anchoredPosition.x >= RightBoundary)
            {
                MovingRight = false;
            }
        }
        else
        {
            RectTransform.anchoredPosition -= new Vector2(step, 0);
            if (RectTransform.anchoredPosition.x <= LeftBoundary)
            {
                MovingRight = true;
            }
        }
    }

    void MoveClawHandDown()
    {
        float step = DownSpeed * Time.deltaTime;
        RectTransform.anchoredPosition -= new Vector2(0, step);

        // Check for ingredients on the way down continuously.
        CheckForIngredient();

        // Check here if the claw hand has reached the floor boundary.
        if (RectTransform.anchoredPosition.y <= DownBoundary)
        {
            // If no ingredient was found, then reset the hand.
            CurrentState = HandState.Resetting;

            HandImage.sprite = PinchedHandSprite;
        }
    }

    void CheckForIngredient()
    {
        foreach (RectTransform IngredientRect in IngredientRects)
        {
            if (RectOverlaps(RectTransform, IngredientRect))
            {
                // If an ingredient is found, pick it up.
                PickUpIngredient(IngredientRect.gameObject);
                IngredientRects.Remove(IngredientRect);
                break;
            }
        }
    }

    bool RectOverlaps(RectTransform RectTransform1, RectTransform RectTransform2)
    {
        Rect Rect1 = GetWorldRect(RectTransform1);
        Rect Rect2 = GetWorldRect(RectTransform2);

        return Rect1.Overlaps(Rect2);
    }

    Rect GetWorldRect(RectTransform RectTransform)
    {
        Vector3[] corners = new Vector3[4];
        RectTransform.GetWorldCorners(corners);

        float XMin = corners[0].x;
        float XMax = corners[2].x;
        float YMin = corners[0].y;
        float YMax = corners[2].y;

        return new Rect(XMin, YMin, XMax - XMin, YMax - YMin);
    }

    void PickUpIngredient(GameObject ingredient)
    {
        // Parent the ingredient to the claw hand to make it "stick" to the claw hand.
        ingredient.transform.SetParent(transform, worldPositionStays: false);
        // Adjust the local position of the ingredient here.
        ingredient.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
        CarriedIngredient = ingredient;

        HandImage.sprite = PinchedHandSprite;

        // Once ingredient is picked up, proceed to the drop-off point.
        CurrentState = HandState.PickingUp;
    }

    void MoveToDropOff()
    {
        Vector2 CurrentPosition = RectTransform.anchoredPosition;
        Vector2 TargetPosition = DropOffPoint.anchoredPosition;

        // First, move vertically upwards to the Y position of the drop-off.
        if (Mathf.Abs(CurrentPosition.y - TargetPosition.y) > 0.1f)
        {
            float VerticalStep = MoveToDropOffSpeed * Time.deltaTime;
            float newY = Mathf.MoveTowards(CurrentPosition.y, TargetPosition.y, VerticalStep);
            RectTransform.anchoredPosition = new Vector2(CurrentPosition.x, newY);
        }
        // Then, move horizontally towards the X position of the drop-off.
        else if (Mathf.Abs(CurrentPosition.x - TargetPosition.x) > 0.1f)
        {
            float HorizontalStep = MoveToDropOffSpeed * Time.deltaTime;
            float NewX = Mathf.MoveTowards(CurrentPosition.x, TargetPosition.x, HorizontalStep);
            RectTransform.anchoredPosition = new Vector2(NewX, CurrentPosition.y);
        }
        else
        {
            DeliverIngredient();
        }
    }

    // Perform drop off, then reset the claw hand.
    void DeliverIngredient()
    {
        if (CarriedIngredient != null)
        {
            Ingredient IngredientComponent = CarriedIngredient.GetComponent<Ingredient>();
            if (IngredientComponent != null)
            {
                IngredientComponent.PickUp();
            }

            Destroy(CarriedIngredient);
            CarriedIngredient = null;
        }

        CurrentState = HandState.Resetting;
    }

    void ResetClawHandPosition()
    {
        Vector2 position = RectTransform.anchoredPosition;
        // First, move vertically upwards to the initial Y position.
        if (Mathf.Abs(position.y - InitialPosition.y) > 0.1f)
        {
            float VerticalStep = ResetSpeed * Time.deltaTime;
            float NewY = Mathf.MoveTowards(position.y, InitialPosition.y, VerticalStep);
            RectTransform.anchoredPosition = new Vector2(position.x, NewY);
        }
        // Then, move horizontally towards the initial X position.
        else if (Mathf.Abs(position.x - InitialPosition.x) > 0.1f)
        {
            float HorizontalStep = ResetSpeed * Time.deltaTime;
            float NewX = Mathf.MoveTowards(position.x, InitialPosition.x, HorizontalStep);
            RectTransform.anchoredPosition = new Vector2(NewX, position.y);
        }
        else
        {
            RectTransform.anchoredPosition = InitialPosition;
            CurrentState = HandState.MovingHorizontally;
            HandImage.sprite = OpenHandSprite;
            SetButtonState(true);
        }
    }

    void OnGrabButtonClicked()
    {
        if (CurrentState == HandState.MovingHorizontally && IsButtonReady)
        {
            CurrentState = HandState.MovingDown;
            SetButtonState(false);
        }
    }

    // Indicate button state using colors.
    void SetButtonState(bool IsReady)
    {
        IsButtonReady = IsReady;
        GrabButton.interactable = IsReady;

        ColorBlock colors = GrabButton.colors;
        colors.normalColor = IsReady ? Color.green : Color.gray;
        colors.highlightedColor = colors.normalColor;
        colors.pressedColor = colors.normalColor;
        colors.disabledColor = colors.normalColor;
        GrabButton.colors = colors;
    }

    void OnGrabAction(InputAction.CallbackContext context)
    {
        if (CurrentState == HandState.MovingHorizontally && IsButtonReady)
        {
            CurrentState = HandState.MovingDown;
            SetButtonState(false);
            // Optional visual feedback.
            GrabButton.OnPointerDown(new PointerEventData(EventSystem.current));
            GrabButton.OnPointerUp(new PointerEventData(EventSystem.current));
        }
    }

    public void DisableInput()
    {
        InputActions.PickTheIngredientsPlayer.Disable();
    }

    private void OnDestroy()
    {
        if (InputActions != null)
        {
            InputActions.PickTheIngredientsPlayer.Grab.performed -= OnGrabAction;
            InputActions.Dispose();
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class HeatKnobController : MonoBehaviour
{
    public float RotationSpeed = 100f;
    public float MinRotation = -85f;
    public float MaxRotation = 85f;

    public delegate void KnobRotatedEventHandler();
    public event KnobRotatedEventHandler OnKnobRotated;

    private PlayerInputActions RotateControls;
    private float CurrentZRotation = 0f;

    private float[] angles = new float[] { 85f, 30f, 0f, -30f, -85f };
    private int CurrentIndex = 2; // Start at 0f.

    private void Awake()
    {
        RotateControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        RotateControls.HeatThePotionPlayer.RotateLeft.performed += OnRotateLeft;
        RotateControls.HeatThePotionPlayer.RotateRight.performed += OnRotateRight;
        RotateControls.HeatThePotionPlayer.Enable();
    }

    private void OnDisable()
    {
        RotateControls.HeatThePotionPlayer.RotateLeft.performed -= OnRotateLeft;
        RotateControls.HeatThePotionPlayer.RotateRight.performed -= OnRotateRight;
        RotateControls.HeatThePotionPlayer.Disable();
    }

    private void OnRotateLeft(InputAction.CallbackContext context)
    {
        CurrentIndex = Mathf.Max(CurrentIndex - 1, 0);
        UpdateRotation();
        OnKnobRotated?.Invoke();
    }

    private void OnRotateRight(InputAction.CallbackContext context)
    {
        CurrentIndex = Mathf.Min(CurrentIndex + 1, angles.Length - 1);
        UpdateRotation();
        OnKnobRotated?.Invoke();
    }

    private void UpdateRotation()
    {
        float angle = angles[CurrentIndex];
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public float GetCurrentZRotation()
    {
        float ZRotation = transform.localEulerAngles.z;
        if (ZRotation > 180f)
            ZRotation -= 360f;
        //Debug.Log("Current Z Rotation: " + ZRotation);
        return ZRotation;
    }
    
    public void ResetKnob()
    {
        CurrentZRotation = 0f;
        transform.localRotation = Quaternion.Euler(0, 0, 0f);
        CurrentIndex = 2;
    }
}

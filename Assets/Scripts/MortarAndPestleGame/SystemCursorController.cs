using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class SystemCursorController : MonoBehaviour
{
    public float CursorSpeed = 1000f;

    private PlayerInputActions PlayerInputActions;
    private Vector2 MoveInput;

    private void Awake()
    {
        PlayerInputActions = new PlayerInputActions();

        // Subscribe to the MoveCursor action here.
        PlayerInputActions.MortarAndPestlePlayer.MoveCursor.performed += OnMoveCursor;
        PlayerInputActions.MortarAndPestlePlayer.MoveCursor.canceled += OnMoveCursorCanceled;

        // And subscribe to the Click action here.
        PlayerInputActions.MortarAndPestlePlayer.MouseClick.performed += OnClickPerformed;
        PlayerInputActions.MortarAndPestlePlayer.MouseClick.canceled += OnClickCanceled;
    }

    private void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void OnDestroy()
    {
        // Unsubscribe from actions on destroy.
        PlayerInputActions.MortarAndPestlePlayer.MoveCursor.performed -= OnMoveCursor;
        PlayerInputActions.MortarAndPestlePlayer.MoveCursor.canceled -= OnMoveCursorCanceled;
        PlayerInputActions.MortarAndPestlePlayer.MouseClick.performed -= OnClickPerformed;
        PlayerInputActions.MortarAndPestlePlayer.MouseClick.canceled -= OnClickCanceled;
    }

    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCursorCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        SimulateLeftClickDown();
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        SimulateLeftClickUp();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCursor();
    }

    private void MoveCursor()
    {
        if (MoveInput == Vector2.zero) return;

        // Get the current position of the mouse.
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 MousePosition = mouse.position.ReadValue();

        // Calculate the new position. Using unscaledDeltaTime works even when the game is paused (Time scale = 0).
        MousePosition += MoveInput * CursorSpeed * Time.unscaledDeltaTime;

        // Clamp the mouse position to the viewport.
        MousePosition.x = Mathf.Clamp(MousePosition.x, 0, Screen.width - 1);
        MousePosition.y = Mathf.Clamp(MousePosition.y, 0, Screen.height - 1);

        // Used to move the mouse cursor.
        Mouse.current.WarpCursorPosition(MousePosition);
    }

    // For the Windows platform.
    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    private void SimulateLeftClickDown()
    {
#if UNITY_STANDALONE_WIN
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
#endif
    }

    private void SimulateLeftClickUp()
    {
#if UNITY_STANDALONE_WIN
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
#endif
    }
}

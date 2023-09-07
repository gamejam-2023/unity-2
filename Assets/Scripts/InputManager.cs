using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    #region Events
    
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;

    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;

    public delegate void UP(float axis);
    public event UP OnUP;

    public delegate void DOWN(float axis);
    public event DOWN OnDOWN;

    public delegate void LEFT(float axis);
    public event LEFT OnLEFT;

    public delegate void RIGHT(float axis);
    public event RIGHT OnRIGHT;

    #endregion

    private Camera mainCamera;

    private TouchAction touchAction;

    private void Awake() {
        touchAction = new TouchAction();
        mainCamera = Camera.main;
    }

    private void OnEnable() {
        touchAction.Enable();
    }

    private void OnDisable() {
        touchAction.Disable();
    }

    void Start()
    {
        touchAction.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        touchAction.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);

        touchAction.Touch.UP.performed += ctx => UPPrimary(ctx);
        touchAction.Touch.DOWN.performed += ctx => DOWNPrimary(ctx);
        touchAction.Touch.LEFT.performed += ctx => LEFTPrimary(ctx);
        touchAction.Touch.RIGHT.performed += ctx => RIGHTPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context) {
        if (OnStartTouch != null) OnStartTouch(Utils.ScreenToWorld(mainCamera, touchAction.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext context) {
        if (OnEndTouch != null) OnEndTouch(Utils.ScreenToWorld(mainCamera, touchAction.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.time);
    }

    private void UPPrimary(InputAction.CallbackContext context) {
        if (OnUP != null) OnUP(touchAction.Touch.UP.ReadValue<float>());
    }

    private void DOWNPrimary(InputAction.CallbackContext context) {
        if (OnDOWN != null) OnDOWN(touchAction.Touch.DOWN.ReadValue<float>());
    }

    private void LEFTPrimary(InputAction.CallbackContext context) {
        if (OnLEFT != null) OnLEFT(touchAction.Touch.LEFT.ReadValue<float>());
    }

    private void RIGHTPrimary(InputAction.CallbackContext context) {
        if (OnRIGHT != null) OnRIGHT(touchAction.Touch.RIGHT.ReadValue<float>());
    }

    public Vector2 PrimaryPosition() {
        return Utils.ScreenToWorld(mainCamera, touchAction.Touch.PrimaryPosition.ReadValue<Vector2>());
    }
}

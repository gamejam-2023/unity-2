using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class VirtualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private Button actionButton;
    [SerializeField] private Canvas canvas;

    [Header("Joystick Settings")]
    [SerializeField] private float joystickRange = 50f;
    [SerializeField] private float deadZone = 0.1f;

    [Header("Portrait Position (bottom center, like fingerprint)")]
    [SerializeField] private Vector2 portraitJoystickAnchor = new Vector2(0.5f, 0.12f);
    [SerializeField] private Vector2 portraitButtonAnchor = new Vector2(0.85f, 0.12f);

    [Header("Landscape Position (left side for thumb)")]
    [SerializeField] private Vector2 landscapeJoystickAnchor = new Vector2(0.15f, 0.25f);
    [SerializeField] private Vector2 landscapeButtonAnchor = new Vector2(0.85f, 0.25f);

    private Vector2 joystickInput;
    private bool isDragging;
    private int dragPointerId = -1;
    private ScreenOrientation lastOrientation;

    public Vector2 JoystickInput => joystickInput;
    public static VirtualController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        bool shouldShow = false;
        
        // Always show on actual mobile builds
        #if UNITY_IOS || UNITY_ANDROID
        shouldShow = true;
        #endif
        
        // In Editor, show when device simulator has touch available
        #if UNITY_EDITOR
        if (Touchscreen.current != null)
        {
            shouldShow = true;
        }
        #endif
        
        if (!shouldShow)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void Start()
    {
        SetupActionButton();
        UpdateLayoutForOrientation();
        lastOrientation = Screen.orientation;
    }

    private void Update()
    {
        // Check for orientation changes
        if (Screen.orientation != lastOrientation)
        {
            lastOrientation = Screen.orientation;
            UpdateLayoutForOrientation();
        }

        HandleJoystickInput();
    }

    private void SetupActionButton()
    {
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(OnActionButtonPressed);
        }
    }

    private void OnActionButtonPressed()
    {
        // Simulate Enter/Space key press for menu selection
        var eventSystem = EventSystem.current;
        if (eventSystem != null && eventSystem.currentSelectedGameObject != null)
        {
            // Don't submit to the action button itself (prevents infinite recursion)
            if (eventSystem.currentSelectedGameObject == actionButton.gameObject)
                return;
                
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, 
                new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
    }

    private void HandleJoystickInput()
    {
        if (joystickBackground == null || joystickHandle == null) return;

        // Handle touch input
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                if (IsTouchOnJoystick(touch.position) && !isDragging)
                {
                    isDragging = true;
                    dragPointerId = touch.fingerId;
                }
            }
            else if (touch.fingerId == dragPointerId)
            {
                if (touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Stationary)
                {
                    UpdateJoystickPosition(touch.position);
                }
                else if (touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled)
                {
                    ResetJoystick();
                }
            }
        }

        // Also handle mouse for editor testing
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && IsTouchOnJoystick(Input.mousePosition))
        {
            isDragging = true;
        }
        if (isDragging && Input.GetMouseButton(0))
        {
            UpdateJoystickPosition(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            ResetJoystick();
        }
        #endif
    }

    private bool IsTouchOnJoystick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, screenPosition, canvas.worldCamera, out Vector2 localPoint);
        
        float radius = joystickBackground.sizeDelta.x * 0.5f;
        return localPoint.magnitude <= radius * 1.5f; // Slightly larger touch area
    }

    private void UpdateJoystickPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, screenPosition, canvas.worldCamera, out Vector2 localPoint);

        // Clamp to joystick range
        Vector2 clampedPoint = Vector2.ClampMagnitude(localPoint, joystickRange);
        joystickHandle.anchoredPosition = clampedPoint;

        // Calculate normalized input
        joystickInput = clampedPoint / joystickRange;
        
        // Apply dead zone
        if (joystickInput.magnitude < deadZone)
        {
            joystickInput = Vector2.zero;
        }
    }

    private void ResetJoystick()
    {
        isDragging = false;
        dragPointerId = -1;
        joystickInput = Vector2.zero;
        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    private void UpdateLayoutForOrientation()
    {
        bool isPortrait = Screen.height > Screen.width;
        
        Vector2 joystickAnchor = isPortrait ? portraitJoystickAnchor : landscapeJoystickAnchor;
        Vector2 buttonAnchor = isPortrait ? portraitButtonAnchor : landscapeButtonAnchor;

        if (joystickBackground != null)
        {
            joystickBackground.anchorMin = joystickAnchor;
            joystickBackground.anchorMax = joystickAnchor;
            joystickBackground.anchoredPosition = Vector2.zero;
        }

        if (actionButton != null)
        {
            RectTransform buttonRect = actionButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = buttonAnchor;
            buttonRect.anchorMax = buttonAnchor;
            buttonRect.anchoredPosition = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

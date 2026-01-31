using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minimumDistance = 50f; // In screen pixels

    private InputManager inputManager;
    private PlayerController playerController;

    private Vector2 touchStartPosition;
    private bool isTouching;

    void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void Awake() {
        inputManager = InputManager.Instance;
    }

    private void OnEnable() {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;

        inputManager.OnUP += UP;
        inputManager.OnDOWN += DOWN;
        inputManager.OnLEFT += LEFT;
        inputManager.OnRIGHT += RIGHT;
    }

    private void OnDisable() {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;

        inputManager.OnUP -= UP;
        inputManager.OnDOWN -= DOWN;
        inputManager.OnLEFT -= LEFT;
        inputManager.OnRIGHT -= RIGHT;
    }

    private void SwipeStart(Vector2 position, float time) {
        touchStartPosition = position;
        isTouching = true;
    }

    private void SwipeEnd(Vector2 position, float time) {
        isTouching = false;
    }

    void Update() {
        // Check for active touch and detect swipe direction continuously
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) {
            Vector2 currentPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector2 delta = currentPos - touchStartPosition;
            
            if (delta.magnitude >= minimumDistance) {
                Vector2 direction = delta.normalized;
                playerController.SetSwipeDirection(direction);
                Debug.Log($"Swipe direction: {direction}");
                touchStartPosition = currentPos; // Reset for continuous swiping
            }
        }
    }

    private void UP(float axis) {
        if (axis == 1) {
            Debug.Log("Keyboard UP");
            playerController.QueueMove(Direction.UP); 
        }
    }

    private void DOWN(float axis) {
        if (axis == 1) {
            Debug.Log("Keyboard DOWN");
            playerController.QueueMove(Direction.DOWN);
        }
    }

    private void LEFT(float axis) {
        if (axis == 1) {
            Debug.Log("Keyboard LEFT");
            playerController.QueueMove(Direction.LEFT);
        }
    }

    private void RIGHT(float axis) {
        if (axis == 1) {
            Debug.Log("Keyboard RIGHT");
            playerController.QueueMove(Direction.RIGHT);
        }
    }
}

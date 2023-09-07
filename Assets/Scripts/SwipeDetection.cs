using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minimumDistance = .2f;

    [SerializeField]
    private float maximumTime = 1f;

    [SerializeField, Range(0f, 1f)]
    private float directionThreshold = .9f;

    [SerializeField]
    private GameObject trail;

    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;

    private Vector2 endPosition;
    private float endTime;

    private Coroutine coroutine;

    private Animator animator;
    private PlayerController playerController;

    public GameOverScreen gameOverScreen;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
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
        startPosition = position;
        startTime = time;

        if (trail != null) {
            trail.SetActive(true);
            trail.transform.position = position;

            coroutine = StartCoroutine(Trail());
        }
    }

    private IEnumerator Trail() {
        while(true) {
            trail.transform.position = inputManager.PrimaryPosition();
            yield return null;
        }
    }

    private void SwipeEnd(Vector2 position, float time) {
        if (trail != null) {
            trail.SetActive(false);
            StopCoroutine(coroutine);
        }

        endPosition = position;
        endTime = time;

        DetectSwipe();
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

    private void DetectSwipe() {
        if (Vector3.Distance(startPosition, endPosition) >= minimumDistance &&
            (endTime - startTime) <= maximumTime) {
                Debug.Log("Swipe Detected");
                Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
        }
    }

    private void SwipeDirection(Vector2 direction) {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold) {
            Debug.Log("Swipe up");

            playerController.QueueMove(Direction.UP);
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold) {
            Debug.Log("Swipe down");

            playerController.QueueMove(Direction.DOWN);
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold) {
            Debug.Log("Swipe left");

            playerController.QueueMove(Direction.LEFT);
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold) {
            Debug.Log("Swipe right");

            playerController.QueueMove(Direction.RIGHT);
        }
    }
}

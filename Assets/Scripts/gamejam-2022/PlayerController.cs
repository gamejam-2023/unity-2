using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    private float _nextAllowedAttack = 0.0f;
    private float _nextAllowedDamage = 0.0f;
    [SerializeField] private float _attackSpeed = 0.6f;
    [SerializeField] private float _invulnerabilityDuration = 5.0f;
    [SerializeField] private float _damage = 10.0f;
    [SerializeField] private float _health = 100;


    [SerializeField] private int walkSpeed = 100;
    private Rigidbody2D body;
    public Animator animator;

    public AudioSource audio;
    public AudioSource audio1;
    public AudioSource ambientAudio0;
    public AudioSource ambientAudio1;
    public AudioSource windAudio;
    public AudioSource lavaAudio;
    public AudioSource gameOverAudio;

    public AudioClip audioWalk;

    public AudioClip waterAudio;
    public AudioClip pestAudio;
    public AudioClip collideAudio;
    public AudioClip growAudio;
    public AudioClip shrinkAudio;

    public Vector2 movement;
    public Vector2 RawInput { get; private set; }
    
    [SerializeField] private float inputSmoothSpeed = 15f;
    private Vector2 smoothedInput;
    private Vector2 lastNonZeroInput;

    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private ShuffleWalkVisual hopVisual;
    [SerializeField] private int speed = 10;
    [SerializeField] private float enemyDetectionRadius = 12f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float cooldownSeconds = 5f;

    private float nextAllowedTime;

    private bool gameOver;
    public bool getGameOver() {
        return gameOver;
    }

    public void setGameOver() {
        Debug.Log("Game over");
        gameOver = true;

        ambientAudio0.volume = 0f;
        ambientAudio1.volume = 0f;
        windAudio.volume = 0f;
        lavaAudio.volume = 0f;

        gameOverAudio.Play();

        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ExecMove() {
        Vector2 keyboardInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (keyboardInput.sqrMagnitude > 1f)
            keyboardInput = keyboardInput.normalized;
        
        // Check virtual controller input
        Vector2 virtualInput = Vector2.zero;
        if (VirtualController.Instance != null)
        {
            virtualInput = VirtualController.Instance.JoystickInput;
        }
        
        // Get target input
        Vector2 targetInput;
        if (keyboardInput.sqrMagnitude > 0.01f) {
            targetInput = keyboardInput;
            lastNonZeroInput = keyboardInput;
        } else if (virtualInput.sqrMagnitude > 0.01f) {
            targetInput = virtualInput;
            lastNonZeroInput = virtualInput;
        } else {
            targetInput = Vector2.zero;
        }
        
        // Smooth the input to prevent snapping on release
        // When stopping, blend towards zero from last direction (not snap to a different direction)
        if (targetInput.sqrMagnitude < 0.01f && smoothedInput.sqrMagnitude > 0.01f)
        {
            // Stopping - smoothly decrease magnitude while keeping direction
            smoothedInput = Vector2.Lerp(smoothedInput, Vector2.zero, inputSmoothSpeed * Time.deltaTime);
        }
        else
        {
            // Moving - smooth towards target
            smoothedInput = Vector2.Lerp(smoothedInput, targetInput, inputSmoothSpeed * Time.deltaTime);
        }
        
        RawInput = smoothedInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (gameOver)
            return;

        HandleEnemyDetection();

        ExecMove(); // sets RawInput

        // Get movement from hop visual (smoothed)
        Vector2 moveDir = Vector2.zero;
        if (hopVisual != null)
        {
            moveDir = hopVisual.MovementDirection;
        }
        else
        {
            // Use RawInput directly - it already has correct magnitude
            // (keyboard gives 1.0, virtual joystick gives 0-1 based on how far pushed)
            moveDir = RawInput;
        }

        // Prevent faster diagonal movement, but preserve magnitude for analog input
        float magnitude = moveDir.magnitude;
        if (magnitude > 1f)
        {
            moveDir = moveDir.normalized;
            magnitude = 1f;
        }

        Vector2 delta = moveDir * speed * Time.fixedDeltaTime;
        Vector2 targetPos = body.position + delta;

        body.MovePosition(targetPos);

        // Update animator with actual movement
        animator.SetFloat("Horizontal", moveDir.x);
        animator.SetFloat("Vertical", moveDir.y);
        animator.SetFloat("Speed", moveDir.sqrMagnitude);

        lavaAudio.volume = Mathf.Abs(body.position.y) / 100f;
    }

    private void HandleEnemyDetection()
    {
        // Cooldown before checking for a new enemy
        if (Time.time < _nextAllowedAttack)
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            body.position,
            enemyDetectionRadius,
            enemyLayer
        );

        if (hits.Length == 0)
        {
            return;
        }

        // Find the closest enemy
        Transform closestEnemy = null;
        float closestSqrDistance = float.MaxValue;
        Vector2 playerPos = body.position;

        foreach (Collider2D hit in hits)
        {
            float sqrDist = ((Vector2)hit.transform.position - playerPos).sqrMagnitude;
            if (sqrDist < closestSqrDistance)
            {
                closestSqrDistance = sqrDist;
                closestEnemy = hit.transform;
            }
        }

        Debug.Log("Closest enemy found: " + closestEnemy.name);

        _nextAllowedAttack = Time.time + _attackSpeed;
        FireAtEnemy(closestEnemy);
    }

    private void FireAtEnemy(Transform enemy)
    {
        Debug.Log("Firing at enemy!");

        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col == null)
        {
            return;
        }

        Vector2 targetPoint = col.bounds.center;
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;

        GameObject proj = Instantiate(
            _projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        proj.GetComponent<Projectile>().Init(direction, _damage);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Collided with " + other.name);
        if (other.CompareTag("Enemy") == true)
        {
            if (other.TryGetComponent(out EnemyScript enemy) && Time.time >= _nextAllowedDamage)
            {
                audio1.clip = pestAudio;
                audio1.Play();
                _health -= (int)enemy.Damage;

                _nextAllowedDamage = Time.time + _invulnerabilityDuration;
            }
        }

        if (_health <= 0f)
        {
            Destroy(gameObject);
            setGameOver();
        }
    }
}

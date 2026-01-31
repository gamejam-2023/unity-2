using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ShootingEnemyScript : EnemyBase
{
    [SerializeField] HealthBar healthBar;
    public bool healthBarVisable = false;
    public bool alwaysShowHealthBar = false;

    // timer of how long to show health bar after taking damage
    float healthBarTimer = 0f;
    float healthBarDisplayDuration = 2f;

    [Header("Enemy Stats")]
    public float Speed = 2f;
    public float Health = 50f;
    public float MaxHealth = 50f;

    [Header("Physics Tuning")]
    public float acceleration = 25f; // how quickly they reach max speed

    [Header("Shooting")]
    public float stopDistance = 6f;          // stop moving when within this distance from player
    public float fireRate = 1.0f;            // shots per second (1 = one shot per second)
    public float projectileDamage = 10f;
    public GameObject projectilePrefab;
    public Transform shootPoint;             // optional: where bullets spawn (defaults to this transform)

    private Transform player;
    private Rigidbody2D rb;

    private float nextShootTime = 0f;

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
            Destroy(gameObject);

        if (alwaysShowHealthBar) return;

        healthBar.updateHealthBar(Health, MaxHealth);
        healthBarVisable = true;
        healthBarTimer = healthBarDisplayDuration;
        healthBar.showHealthBar();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        HealthBar _healthBar = FindFirstObjectByType<HealthBar>();

        if (playerObj != null)
            player = playerObj.transform;

        if (_healthBar != null)
            healthBar = _healthBar;

        if (healthBar != null)
        {
            healthBar.updateHealthBar(Health, MaxHealth);
            if (alwaysShowHealthBar)
            {
                healthBarVisable = true;
                healthBar.showHealthBar();
            }
            else
            {
                healthBarVisable = false;
                healthBar.hideHealthBar();
            }
        }

        if (shootPoint == null)
            shootPoint = transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        float dist = toPlayer.magnitude;

        // If far away -> move towards player
        if (dist > stopDistance)
        {
            if (dist < 0.0001f) return;

            Vector2 dir = toPlayer / dist; // normalized
            Vector2 targetVel = dir * Speed;

            // Smooth acceleration towards target velocity
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Within stop range -> stop moving
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, acceleration * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Health bar timer logic
        if (healthBarVisable && !alwaysShowHealthBar && healthBar != null)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0f)
            {
                healthBarVisable = false;
                healthBar.hideHealthBar();
            }
        }

        // Shooting logic (only shoot when within stop distance)
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer <= stopDistance)
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (projectilePrefab == null) return;
        if (fireRate <= 0f) return;
        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + (1f / fireRate);
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.Init((player.position - shootPoint.position).normalized);
        }
    }
}
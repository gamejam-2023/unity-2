using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : EnemyBase
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
    public float Damage = 10f;

    [Header("Physics Tuning")]
    public float acceleration = 25f; // how quickly they reach max speed

    private Transform player;
    private Rigidbody2D rb;

    public void TakeDamage(float damage)
    {
        Debug.Log("Enemy took " + damage + " damage.");
        Health -= damage;
        if (Health <= 0f)
            Destroy(gameObject);
        
        if (alwaysShowHealthBar) return;
        healthBar.updateHealthBar(Health, MaxHealth);
        healthBarVisable = true;
        healthBarTimer = healthBarDisplayDuration; // show health bar for 2 seconds
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

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 dir = (Vector2)player.position - rb.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();

        // Target velocity (simple + stable)
        Vector2 targetVel = dir * Speed;

        // Smooth acceleration towards target velocity
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, acceleration * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (healthBarVisable && !alwaysShowHealthBar)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0f)
            {
                healthBarVisable = false;
                healthBar.hideHealthBar();
            }
        }
    }

    
}

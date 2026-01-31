using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;
    public float Speed = 2f;
    public float Health = 50f;
    public float MaxHealth = 50f;
    public float Damage = 10f;
    public float TimeToStartSpawning = 0f; // after how many seconds should this start spawning

    [Header("Physics Tuning")]
    public float acceleration = 25f; // how quickly they reach max speed

    private Transform player;
    private Rigidbody2D rb;

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
            Destroy(gameObject);
        
        healthBar.updateHealthBar(Health, MaxHealth);
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
        
    }
}

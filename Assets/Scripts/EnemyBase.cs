using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;

    public bool healthBarVisable = false;
    public bool alwaysShowHealthBar = false;
    public float TimeToStartSpawning = 0f;
    public float TimeToEndSpawning = 60f;
    public int ScoreValue = 100;

    float healthBarTimer = 0f;
    float healthBarDisplayDuration = 2f;
    
    [Header("Enemy Stats")]
    public float Speed = 2f;
    public float Health = 50f;
    public float MaxHealth = 50f;

    [Header("Physics Tuning")]
    public float acceleration = 25f; // how quickly they reach max speed

    public Transform player;

    private GameStates gameStates;

    public Rigidbody2D rb;

    void Awake()
    {
        gameStates = FindFirstObjectByType<GameStates>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

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
    }

    public virtual void Update()
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
    }

    void OnDestroy()
    {
        if (gameStates)
        {
            gameStates.score += ScoreValue;
        }
    }
}
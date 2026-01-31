using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : MonoBehaviour
{
    public float speed = 2f;
    public float health = 50f;

    [Header("Physics Tuning")]
    public float acceleration = 25f; // how quickly they reach max speed

    private Transform player;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();

        // Target velocity (simple + stable)
        Vector2 targetVel = dir * speed;

        // Smooth acceleration towards target velocity
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, acceleration * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (health <= 0f)
            Destroy(gameObject);
    }
}

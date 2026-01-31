using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ShootingEnemyScript : EnemyBase
{


    [Header("Shooting")]
    public float stopDistance = 6f;          // stop moving when within this distance from player
    public float fireRate = 1.0f;            // shots per second (1 = one shot per second)
    public float projectileDamage = 10f;
    public GameObject projectilePrefab;
    public Transform shootPoint;             // optional: where bullets spawn (defaults to this transform)

    private float nextShootTime = 0f;

    void Start()
    {
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

    public override void Update()
    {
        // Shooting logic (only shoot when within stop distance)
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer <= stopDistance)
        {
            TryShoot();
        }
        base.Update();
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
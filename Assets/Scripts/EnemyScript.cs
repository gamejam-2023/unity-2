using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyScript : EnemyBase
{
    // timer of how long to show health bar after taking damage

    public float Damage = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    public override void Update()
    {
        // Nothing for now
        base.Update();
    }

    
}

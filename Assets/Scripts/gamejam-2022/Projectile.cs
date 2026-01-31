using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _lifetime = 3f;

    private Vector2 direction;

    public void Init(Vector2 dir, float damage)
    {
        direction = dir.normalized;
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") == false)
        {
            return;
        }

        if (other.TryGetComponent(out EnemyScript enemy))
        {
            enemy.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}

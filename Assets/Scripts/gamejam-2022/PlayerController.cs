using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 Movement;
    public int Health = 100;
    public float Damage = 10.0f;

    [SerializeField]
    private Rigidbody2D _body;
    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private int _movementSpeed = 10;
    [SerializeField]
    private float _detectionRadius = 12f;
    [SerializeField]
    private float _attackSpeed = 5f;
    [SerializeField]
    private float _invulnerabilityDuration = 5.0f;

    [SerializeField] 
    private GameObject _projectilePrefab;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private AudioSource _audio1;
    [SerializeField] 
    private AudioSource _audio2;
    [SerializeField] 
    private AudioSource _ambientAudio0;
    [SerializeField] 
    private AudioSource _ambientAudio1;
    [SerializeField] 
    private AudioSource _windAudio;
    [SerializeField] 
    private AudioSource _lavaAudio;
    [SerializeField]  
    private AudioSource _gameOverAudio;

    [SerializeField] 
    private AudioClip _audioWalk;
    [SerializeField] 
    private AudioClip _pestAudio;
    [SerializeField] 
    private AudioClip _collideAudio;

    private float _nextAllowedAttack = 0.0f;
    private float _nextAllowedDamage = 0.0f;
    private bool _gameOver = false;

    public void SetGameOver() {
        Debug.Log("Game over");
        _gameOver = true;

        _ambientAudio0.volume = 0f;
        _ambientAudio1.volume = 0f;
        _windAudio.volume = 0f;
        _lavaAudio.volume = 0f;

        _gameOverAudio.Play();
    }

    public void ExecMove() {
        Movement.x = Input.GetAxisRaw("Horizontal");
        Movement.y = Input.GetAxisRaw("Vertical");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy") == true)
        {
            if (other.TryGetComponent(out EnemyScript enemy) && Time.time >= _nextAllowedDamage)
            {
                _audio1.clip = _pestAudio;
                _audio1.Play();
                Health -= (int)enemy.Damage;

                _nextAllowedDamage = Time.time + _invulnerabilityDuration;
            }
        }

        if (Health <= 0f)
        {
            Destroy(gameObject);
            SetGameOver();
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        // Cooldown before checking for a new enemy
        if (Time.time >= _nextAllowedAttack)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                _body.position,
                _detectionRadius,
                _enemyLayer
            );

            if (hits.Length > 0)
            {
                // Pick the first or closest enemy
                Transform currentEnemy = hits[0].transform;
                _nextAllowedAttack = Time.time + _attackSpeed;

                FireAtEnemy(currentEnemy);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_gameOver)
            return;

        ExecMove(); // must set `movement`

        Vector2 input = Movement;

        // Prevent faster diagonal movement
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector2 delta = input * _movementSpeed * Time.fixedDeltaTime;
        Vector2 targetPos = _body.position + delta;

        _body.MovePosition(targetPos);

        _animator.SetFloat("Horizontal", input.x);
        _animator.SetFloat("Vertical", input.y);
        _animator.SetFloat("Speed", input.sqrMagnitude);

        _lavaAudio.volume = Mathf.Abs(_body.position.y) / 100f;
    }

    private void FireAtEnemy(Transform enemy)
    {
        Vector2 direction = (enemy.position - transform.position).normalized;

        GameObject proj = Instantiate(
            _projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        proj.GetComponent<Projectile>().Init(direction);
    }
}

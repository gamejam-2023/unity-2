using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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
    private int _health = 100;
    [SerializeField]
    private float _damage = 10.0f;

    [SerializeField]
    private Animator animator;

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
    private AudioClip _waterAudio;
    [SerializeField] 
    private AudioClip _pestAudio;
    [SerializeField] 
    private AudioClip _collideAudio;
    [SerializeField] 
    private AudioClip _growAudio;
    [SerializeField] 
    private AudioClip _shrinkAudio;

    private Vector2 _movement;
    private float _nextAllowedTime = 0.0f;
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
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");
    }

    //TODO handle getting hit by enemies and losing HP
    void OnTriggerEnter2D(Collider2D other) {
        var name = other.gameObject.name;

        Debug.Log($"Colliding with {name}");
        if (name.Contains("item-")) {
            if (name.Contains("Sprites/ggj-2023/Mock-up drop")) {
                if (!_audio1.isPlaying) {
                    _audio1.clip = _waterAudio;
                    _audio1.Play();
                }
            }
            else if (name.Contains("Sprites/ggj-2023/Mock-up pests")) {
                if (!_audio1.isPlaying) {
                    _audio1.clip = _pestAudio;
                    _audio1.Play();
                }
            }

            Destroy(other.gameObject);
        }
        else {
            if (!_audio1.isPlaying) {
                _audio1.clip = _collideAudio;
                _audio1.Play();
            }

            if (name.Contains("Trail") || name.Contains("collider-bottom")) {
                SetGameOver();
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {
        // Cooldown before checking for a new enemy
        if (Time.time >= _nextAllowedTime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                _body.position,
                _detectionRadius,
                _enemyLayer
            );

            // Debug.Log($"Detected {hits.Length} enemies nearby.");

            if (hits.Length > 0)
            {
                // Pick the first or closest enemy
                Transform currentEnemy = hits[0].transform;
                lineActive = true;
                _nextAllowedTime = Time.time + _attackSpeed;
            }
        }

        // Update line every frame if active

    }

    void FixedUpdate()
    {
        if (_gameOver)
            return;

        ExecMove(); // must set `movement`

        Vector2 input = _movement;

        // Prevent faster diagonal movement
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector2 delta = input * _movementSpeed * Time.fixedDeltaTime;
        Vector2 targetPos = _body.position + delta;

        _body.MovePosition(targetPos);

        animator.SetFloat("Horizontal", input.x);
        animator.SetFloat("Vertical", input.y);
        animator.SetFloat("Speed", input.sqrMagnitude);

        _lavaAudio.volume = Mathf.Abs(_body.position.y) / 100f;
    }
}

using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float TimeToStartSpawning = 0f;
    public float TimeToEndSpawning = 60f;
    public int ScoreValue = 100;
    private GameStates gameStates;

    void Awake()
    {
        gameStates = FindFirstObjectByType<GameStates>();
    }

    void OnDestroy()
    {
        gameStates.score += ScoreValue;
    }
}
using TMPro;
using UnityEngine;

public class ScoreValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    private GameStates gameStates;
    // Update is called once per frame

    void Awake()
    {
        gameStates = FindFirstObjectByType<GameStates>();
    }
    
    void Update()
    {
        scoreText.text = gameStates.score.ToString();
    }

}

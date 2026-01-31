using UnityEngine;

public class GameStates : MonoBehaviour
{

    public int score = 0;
    public float gameTime = 0f;
    private float oldGameTime = 0f;
    private int oldScoreCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        gameTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
        // for every second passed, increase score by 1
        if ((int)gameTime > (int)oldGameTime)
        {
            // for every 10 seconds passed, increase score by an additional 1
            oldScoreCounter += 1;
            int extraScoreFromTime = ((int)gameTime / 10) - oldScoreCounter;
            score += 1 + extraScoreFromTime;
            oldGameTime = gameTime;
        }

    }
}

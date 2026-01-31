using UnityEngine;
using TMPro;

public class ExpGainValue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI expValueText;
    private GameStates gameStates;
    // Update is called once per frame

    void Awake()
    {
        gameStates = FindFirstObjectByType<GameStates>();
    }
    
    void Update()
    {
        if (expValueText?.text != null)
        {
            expValueText.text = gameStates.exp.ToString();
        }
    }
}

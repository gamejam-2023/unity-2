using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void updateHealthBar(float health, float maxHealth)
    {
        slider.value = health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

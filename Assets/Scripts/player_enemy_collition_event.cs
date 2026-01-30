using UnityEngine;
using UnityEngine.SceneManagement;

public class player_enemy_collition_event : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // var name = collision.gameObject.name;
        // Debug.Log($"Colliding with {name}2");
        // Debug.Log(collision.gameObject.Tag);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit by enemy!"); 

            SceneManager.LoadScene("EndGame");
        }
    }

}

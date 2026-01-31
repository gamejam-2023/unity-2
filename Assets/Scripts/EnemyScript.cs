using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 2f;
    public float health = 50f;
    private Transform player;


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
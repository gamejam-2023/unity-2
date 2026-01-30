using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 1f;
    public GameObject[] enemyPrefabs;
    public float outsideMargin = 2f;   // How far outside the camera to spawn

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (Camera.main == null) return;

        Vector2 spawnPosition = GetPositionOutsideCamera();

        GameObject prefab =
            enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    Vector2 GetPositionOutsideCamera()
    {
        Camera cam = Camera.main;

        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float left = bl.x;
        float right = tr.x;
        float bottom = bl.y;
        float top = tr.y;

        left -= outsideMargin;
        right += outsideMargin;
        bottom -= outsideMargin;
        top += outsideMargin;

        int side = Random.Range(0, 4);

        return side switch
        {
            0 => new Vector2(left, Random.Range(bottom, top)),   // Left
            1 => new Vector2(right, Random.Range(bottom, top)),  // Right
            2 => new Vector2(Random.Range(left, right), bottom), // Bottom
            _ => new Vector2(Random.Range(left, right), top),    // Top
        };
    }
}
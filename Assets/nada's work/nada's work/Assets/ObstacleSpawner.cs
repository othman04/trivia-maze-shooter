using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // array instead of single prefab

    public Transform player;

    public float spawnRate = 1f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        float randomX = Random.Range(-5f, 5f);

        Vector3 spawnPosition = new Vector3(
            player.position.x + randomX,
            player.position.y + 10f,
            0
        );

        // randomly pick one of your obstacle prefabs
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}
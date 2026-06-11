using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float speedBonus = 2f;
    public int scoreThreshold = 30;
    private Transform player;
    private float currentSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentSpeed = GameManager.Instance.GetCurrentSpeed();
        baseSpeed = currentSpeed;
    }

    void Update()
    {
        if (ScoreManager.score >= scoreThreshold)
            currentSpeed = baseSpeed + speedBonus;
        else
            currentSpeed = baseSpeed;

        transform.position += Vector3.down * currentSpeed * Time.deltaTime;

        if (transform.position.y < player.position.y - 10f)
        {
            ScoreManager.AddScore(transform.position);
            Destroy(gameObject);
        }
    }
}
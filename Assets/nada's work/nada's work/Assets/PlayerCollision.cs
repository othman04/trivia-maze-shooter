using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameOverUI gameOverUI;

    void Start()
    {
        gameOverUI = FindObjectOfType<GameOverUI>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameOverUI != null)
            gameOverUI.Show();
    }
}
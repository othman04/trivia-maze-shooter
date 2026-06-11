using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TextMeshProUGUI scoreText;
    public static ScoreManager instance;

    public GameObject popupPrefab;
    public Canvas canvas;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        score = 0;
        UpdateScoreUI();
    }

    public static void AddScore(Vector3 obstaclePosition)
    {
        score++;

        if (instance != null && instance.popupPrefab != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obstaclePosition);
            GameObject popup = Instantiate(instance.popupPrefab, instance.canvas.transform);
            popup.transform.position = screenPos;
        }

        instance.CheckWin();
    }

    void CheckWin()
    {
        if (score >= 30)
        {
            FindObjectOfType<LevelCompleteUI>(true).Show();
        }
    }

    void Update()
    {
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
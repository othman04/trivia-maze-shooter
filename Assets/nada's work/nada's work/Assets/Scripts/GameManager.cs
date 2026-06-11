using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLevel = 1;
    public float baseSpeed = 3f;
    public float speedPerLevel = 1.2f;
    public int scoreToWin = 30;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public float GetCurrentSpeed()
    {
        return baseSpeed * Mathf.Pow(speedPerLevel, currentLevel - 1);
    }
}

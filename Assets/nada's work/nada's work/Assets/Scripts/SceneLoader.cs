using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void LoadEndScreen()
    {
        SceneManager.LoadScene(6);
    }

    public static void ReloadCurrent()
    {
        LevelStartUI.skipPanel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
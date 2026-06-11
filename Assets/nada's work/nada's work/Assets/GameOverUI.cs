using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    public void Show()
    {
        // ✅ Show cursor so player can click Retry/Main Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnRetry()
    {
        // ✅ Hide cursor when game restarts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Time.timeScale = 1f;
        SceneLoader.ReloadCurrent();
    }

    public void OnMainMenu()
    {
        // ✅ Keep cursor visible for main menu navigation
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Time.timeScale = 1f;
        SceneLoader.LoadMainMenu();
    }
}
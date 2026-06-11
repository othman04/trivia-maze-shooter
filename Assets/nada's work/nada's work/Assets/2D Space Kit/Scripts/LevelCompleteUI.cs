using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    public void Show()
    {
        // ✅ Show cursor so player can click Retry/Next
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Time.timeScale = 0f;
        panel.SetActive(true);
    }

    public void OnNextPressed()
    {
        // ✅ Hide cursor when going back to game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Time.timeScale = 1f;

        if (GameManager.Instance.currentLevel >= 2)
        {
            SceneManager.LoadScene("ShipInterior");
        }
        else
        {
            GameManager.Instance.currentLevel = 2;
            SceneLoader.LoadLevel(2);
        }
    }
}
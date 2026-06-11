using UnityEngine;
using TMPro;

public class LevelStartUI : MonoBehaviour
{
    public static bool skipPanel = false;

    [SerializeField] TextMeshProUGUI levelLabel;
    [SerializeField] GameObject panel;

    void Start()
    {
        if (skipPanel)
        {
            skipPanel = false;
            panel.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        // ✅ Show cursor so player can click Start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Time.timeScale = 0f;
        levelLabel.text = "LEVEL " + GameManager.Instance.currentLevel;
        panel.SetActive(true);
    }

    public void OnStartPressed()
    {
        // ✅ Hide cursor when game starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
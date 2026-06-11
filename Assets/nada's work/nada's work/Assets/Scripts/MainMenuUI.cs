using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.currentLevel = 1;
        SceneLoader.LoadLevel(1);
    }
}
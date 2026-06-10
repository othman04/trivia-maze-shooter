using UnityEngine;
using UnityEngine.SceneManagement; // Requis pour changer de scène

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Charge directement la scène de jeu par son nom
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        // Optionnel : Tu pourras lier un panneau d'options ici plus tard
        Debug.Log("Options Menu Opened!");
    }

    public void QuitGame()
    {
        Debug.Log("Game Exited!");
        Application.Quit(); // Ferme le jeu (une fois le build exporté en .exe)
    }
}
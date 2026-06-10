using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vie")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Interface Mort")]
    public GameObject gameOverPanel; // Ton UI (Panel) de mort
    public Button restartButton;     // Ton bouton pour recommencer

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartScene);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Vie du joueur : " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Débloquer le curseur pour cliquer sur le bouton
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Désactiver le mouvement du joueur pour éviter qu'il bouge en étant mort
        var movementScript = GetComponent<MonoBehaviour>(); 
        if (movementScript != null) movementScript.enabled = false;
    }

    public void RestartScene()
    {
        // Recharge le niveau actuel
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
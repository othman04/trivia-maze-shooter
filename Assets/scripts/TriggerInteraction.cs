using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public string  sceneToLoad = "scene1";
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject promptUI;

    private bool playerInside = false;
    private GameObject player;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            if (player != null)
            {
                ReturnPositionData.savedPosition  = player.transform.position;
                ReturnPositionData.hasReturnPoint = true;
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player       = other.gameObject;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}
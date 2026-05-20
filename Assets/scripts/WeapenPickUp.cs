using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // <-- AJOUT : Nécessaire si vous utilisez TextMeshPro (si texte classique, utilisez: using UnityEngine.UI;)

public class WeaponPickup : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject weaponOnPlayer; // L'arme cachée sous la Main Camera
    
    [Header("UI Interface")]
    public GameObject pickupText; // Le texte "PickupMessage" créé à l'étape 1

    private bool isPlayerNearby = false;

    void Update()
    {
        // Si le joueur est proche et appuie sur E
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    void PickUp()
    {
        if (weaponOnPlayer != null)
        {
            weaponOnPlayer.SetActive(true); // Active l'arme du joueur
            
            if (pickupText != null)
            {
                pickupText.SetActive(false); // Cache le texte car l'arme est ramassée
            }

            Destroy(gameObject); // Détruit l'arme au sol
        }
    }

    // Le joueur entre dans la zone : on affiche le message
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (pickupText != null)
            {
                pickupText.SetActive(true); // Affiche le texte "Pressez E"
            }
        }
    }

    // Le joueur sort de la zone : on cache le message
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (pickupText != null)
            {
                pickupText.SetActive(false); // Cache le texte
            }
        }
    }
}
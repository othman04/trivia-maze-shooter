using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Reference")]
    public Gun gun;

    private bool isHoldingShoot = false;

    // REFACTORISATION : On utilise directement le composant PlayerInput s'il est présent,
    // ou une détection manuelle plus robuste.
    void Update()
    {
        // Méthode de secours ultra-fiable si "OnShoot" ne reçoit pas le relâchement de touche :
        // Elle vérifie directement si l'action "Shoot" de votre Input System est pressée en temps réel.
        if (PlayerInput.all.Count > 0)
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                // Remplacez "Shoot" par le nom exact de votre action dans votre Input Action Asset
                isHoldingShoot = playerInput.actions["Shoot"].IsPressed();
            }
        }

        // Si le joueur reste appuyé et que l'arme existe, on tire
        if (isHoldingShoot && gun != null)
        {
            gun.Shoot();
        }
    }
}
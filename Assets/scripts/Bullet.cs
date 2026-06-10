using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 60f;      // Vitesse rapide et tendue pour éviter la lenteur
    public float lifeTime = 3f;    // Temps avant destruction si la balle ne touche rien
    public float damage = 25f;     // Dégâts infligés à la cible

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Configuration de sécurité pour que la balle aille parfaitement tout droit
        if (rb != null)
        {
            rb.useGravity = false; // Désactive la gravité pour que la balle ne plonge pas vers le sol
            rb.linearDamping = 0f;          // Supprime la friction de l'air pour garder une vitesse constante
        }
    }

    // Fonction appelée par le script Gun (du Joueur ou de l'Ennemi) lors du tir
    public void Launch(Vector3 directionDuTir)
    {
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = false;
            
            // Applique la vitesse ultra-rapide dans la direction du canon (normalisée pour éviter les bugs)
            rb.linearVelocity = directionDuTir.normalized * speed;
            
            // Force l'alignement visuel de la balle (le Pivot) vers sa direction de vol
            transform.forward = directionDuTir.normalized; 
        }

        // Détruit l'objet après quelques secondes pour économiser la mémoire du jeu
        Destroy(gameObject, lifeTime);
    }

    // Détection si le Collider de la balle est configuré en mode classique (Friction physique)
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    // Détection si le Collider de la balle est configuré en mode "Is Trigger" (Fantôme/Détecteur)
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    // Gestion unique et centralisée des impacts de balles
    void HandleCollision(GameObject hitObject)
    {
        // CAS 1 : La balle touche un Ennemi
        if (hitObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Inflige les dégâts à l'ennemi
            }
        }

        // CAS 2 : La balle de l'ennemi touche le Joueur
        if (hitObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Inflige les dégâts au joueur (Écran de mort)
            }
        }

        // Destruction de la balle après l'impact
        // Sécurité : on détruit la balle sauf si elle touche par erreur le pistolet du tireur au démarrage
        if (!hitObject.CompareTag("Player") || hitObject != transform.gameObject)
        {
            Destroy(gameObject);
        }
    }
}
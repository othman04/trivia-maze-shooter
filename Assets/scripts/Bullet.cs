using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 25f; // <-- AJOUT : Dégâts infligés par la balle

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 directionDuTir)
    {
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = directionDuTir * speed;
            transform.forward = directionDuTir; 
        }

        Destroy(gameObject, lifeTime);
    }

    // Si votre collider de balle N'EST PAS en "Is Trigger"
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    // Si votre collider de balle EST en "Is Trigger"
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    // Gestion unique des impacts
    void HandleCollision(GameObject hitObject)
    {
        // On vérifie si l'objet touché est un ennemi
        if (hitObject.CompareTag("Enemy"))
        {
            // On essaie de récupérer le script de vie sur l'ennemi
            EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                // On lui inflige les dégâts !
                enemyHealth.TakeDamage(damage);
            }
        }

        // On détruit la balle après l'impact (sauf si elle touche le joueur qui l'a tirée)
        if (!hitObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
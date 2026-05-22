using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 60f; // <-- CORRECTION : Augmenté à 60f pour un tir rapide et tendu
    public float lifeTime = 3f;
    public float damage = 25f; 

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // SÉCURITÉ : Configure le Rigidbody pour qu'il aille tout droit sans tomber
        if (rb != null)
        {
            rb.useGravity = false; // Empêche la balle de plonger vers le sol immédiatement
            rb.linearDamping = 0f;          // Supprime la friction de l'air pour ne pas ralentir
        }
    }

    public void Launch(Vector3 directionDuTir)
    {
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.isKinematic = false;
            
            // On applique la vitesse ultra-rapide dans la direction donnée par le canon
            rb.linearVelocity = directionDuTir.normalized * speed;
            
            // Aligne la balle visuellement vers sa direction de vol
            transform.forward = directionDuTir.normalized; 
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void HandleCollision(GameObject hitObject)
    {
        if (hitObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = hitObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        if (!hitObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
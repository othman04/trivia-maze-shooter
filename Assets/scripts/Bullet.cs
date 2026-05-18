using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 20f;
    public float lifeTime = 3f;

    private Rigidbody rb;

    void Awake()
    {
        // On récupère le Rigidbody tôt (Awake) car Start ne sera pas
        // appelé tant que la balle n'est pas activée.
        rb = GetComponent<Rigidbody>();
    }

    // Cette fonction sera appelée par le script Gun lors du tir
    public void Launch()
    {
        // 1. On active l'objet
        gameObject.SetActive(true);

        // 2. On désactive la physique pour la phase de tir initial (évite les collisions immédiates)
        if (rb != null)
        {
            rb.isKinematic = false; // Assurez-vous qu'il est en mode dynamique
            rb.linearVelocity = transform.forward * speed; // CORRIGÉ : forward (pas -forward)
        }

        // 3. On programme sa destruction
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Vous pouvez ajouter des effets de particules ou des sons ici
        Destroy(gameObject);
    }
}
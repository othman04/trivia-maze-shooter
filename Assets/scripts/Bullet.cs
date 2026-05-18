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
    public void Launch(Vector3 directionDuTir)
{
    gameObject.SetActive(true);

    if (rb != null)
    {
        rb.isKinematic = false;
        
        // CORRECTION : On force la direction de la vitesse avec la vraie direction du canon, 
        // et on aligne le visuel de la balle pour qu'il regarde dans cette direction.
        rb.linearVelocity = directionDuTir * speed;
        transform.forward = directionDuTir; 
    }

    Destroy(gameObject, lifeTime);
}

    private void OnCollisionEnter(Collision collision)
    {
        // Vous pouvez ajouter des effets de particules ou des sons ici
        Destroy(gameObject);
    }
}
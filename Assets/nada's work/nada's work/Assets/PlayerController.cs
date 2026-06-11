using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float forwardSpeed = 6f;
    public float leftLimit = -8f;
    public float rightLimit = 8f;

    private Rigidbody2D rb;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Remplace GetAxisRaw("Horizontal") qui cause l'erreur
        horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
    }

    void FixedUpdate()
    {
        Vector2 forwardMovement = Vector2.up * forwardSpeed;
        Vector2 sideMovement = Vector2.right * horizontalInput * moveSpeed;

        Vector2 newPosition = rb.position + (forwardMovement + sideMovement) * Time.fixedDeltaTime;

        // Clamp pour rester dans les limites
        newPosition.x = Mathf.Clamp(newPosition.x, leftLimit, rightLimit);

        rb.MovePosition(newPosition);
    }
}
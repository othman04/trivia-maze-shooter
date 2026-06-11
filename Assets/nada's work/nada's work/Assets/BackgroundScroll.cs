using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public SpriteRenderer spriteRenderer;
    private float height;
    private Transform clone;

    void Start()
    {
        height = spriteRenderer.bounds.size.y;

        // create clone above
        clone = Instantiate(gameObject, transform.position + Vector3.up * height, Quaternion.identity).transform;
        clone.GetComponent<BackgroundScroll>().enabled = false;
    }

    void Update()
    {
        // move both down
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        clone.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // when main goes below screen, jump above clone
        if (transform.position.y + height < Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            transform.position = clone.position + Vector3.up * height;
        }

        // when clone goes below screen, jump above main
        if (clone.position.y + height < Camera.main.transform.position.y - Camera.main.orthographicSize)
        {
            clone.position = transform.position + Vector3.up * height;
        }
    }
}
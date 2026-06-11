using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float fadeSpeed = 1.5f;
    private TextMeshProUGUI text;
    private Color color;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        color = text.color;
    }

    void Update()
    {
        // float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // fade out
        color.a -= fadeSpeed * Time.deltaTime;
        text.color = color;

        if (color.a <= 0)
            Destroy(gameObject);
    }
}
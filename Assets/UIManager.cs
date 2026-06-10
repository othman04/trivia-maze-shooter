using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Progression Bar")]
    public Slider          progressionBar;
    public Image           fillImage;
    public Image           humanZoneIndicatorLeft;
    public Image           humanZoneIndicatorRight;
    public TextMeshProUGUI percentageText;

    [Header("Colors")]
    public Color dangerColor  = new Color(1f, 0.2f, 0.2f);
    public Color humanColor   = new Color(0.2f, 1f, 0.8f);
    public Color warningColor = new Color(1f, 0.8f, 0.1f);

    [Header("AXIOM Panel")]
    public Image           axiomGlowImage;
    public TextMeshProUGUI axiomNameText;

    [Header("Glow Pulse Settings")]
    public float pulseSpeed = 2f;
    public float minAlpha   = 0.4f;
    public float maxAlpha   = 1f;

    [Header("Flash Feedback")]
    public float flashDuration = 0.15f;

    // -------------------------------------------------------

    private float currentScore = 0.5f;
    private bool  isPulsing    = true;

    // -------------------------------------------------------

    void Start()
    {
        if (progressionBar != null)
        {
            progressionBar.minValue = 0f;
            progressionBar.maxValue = 1f;
            progressionBar.value    = currentScore;
        }
        UpdateBarVisuals(currentScore);
    }

    void Update()
    {
        if (isPulsing && axiomGlowImage != null)
            PulseGlow();
    }

    // -------------------------------------------------------

    public void UpdateScore(float delta)
    {
        currentScore = Mathf.Clamp01(currentScore + delta);
        if (progressionBar != null)
            progressionBar.value = currentScore;

        StartCoroutine(FlashFeedback(delta > 0));
    }

    public void SetScore(float value)
    {
        currentScore = Mathf.Clamp01(value);
        if (progressionBar != null)
            progressionBar.value = currentScore;
        UpdateBarVisuals(currentScore);
    }

    public float GetScore() => currentScore;

    // -------------------------------------------------------

    private IEnumerator FlashFeedback(bool positive)
    {
        // Right answer = vivid green flash / Wrong answer = vivid red flash
        Color flashColor = positive
            ? new Color(0f,  1f,  0.3f)   // bright green
            : new Color(1f,  0f,  0.1f);  // bright red

        if (fillImage != null)
            fillImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        UpdateBarVisuals(currentScore);
    }

    // -------------------------------------------------------

    private void UpdateBarVisuals(float score)
    {
        // Low score = red, mid = yellow, high score = green
        Color targetColor;
        if (score <= 0.50f)
            targetColor = Color.Lerp(dangerColor, warningColor, score / 0.50f);
        else
            targetColor = Color.Lerp(warningColor, humanColor, (score - 0.50f) / 0.50f);

        if (fillImage != null)
            fillImage.color = targetColor;

        if (percentageText != null)
            percentageText.text = $"{Mathf.RoundToInt(score * 100)}%";

        if (axiomGlowImage != null)
            axiomGlowImage.color = new Color(targetColor.r, targetColor.g,
                                             targetColor.b, axiomGlowImage.color.a);
    }

    private void PulseGlow()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                      (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        Color c = axiomGlowImage.color;
        axiomGlowImage.color = new Color(c.r, c.g, c.b, alpha);
    }
}
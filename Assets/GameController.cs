using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public UIManager uiManager;

    [Header("Scene Settings")]
    public string returnScene = "ShipInterior"; // ✅ Set this in Inspector per scene

    [Header("Dialogue UI")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI endingText;
    public GameObject      choicePanel;
    public GameObject      axiomPanel;

    [Header("Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("Button Labels")]
    public TextMeshProUGUI btn1Text;
    public TextMeshProUGUI btn2Text;
    public TextMeshProUGUI btn3Text;

    // -------------------------------------------------------

    protected string[] questions = {
        "A CAPTCHA asks if you're human.",
        "Which is more human?",
        "Why do people keep embarrassing memories forever?",
        "Why do humans maintain friendships?",
        "Rachel has chosen to remain alone at home for the evening and drink two litres of red wine.\n\nPlease identify the correct chemical formula for consumable alcohol. Is it:"
    };

    protected string[][] answers = {
        new string[] { "Feel weirdly offended",      "Complete the task accurately",        "Begin existential crisis" },
        new string[] { "Making mistakes repeatedly",  "Being correct all the time",          "Eliminating inefficiency completely" },
        new string[] { "The brain enjoys torture",    "Emotional memory has high retention", "Permanent cringe archive initialized" },
        new string[] { "Social cooperation",          "Emotional support",                   "To send each other memes instead of discussing the terrifying reality of existence." },
        new string[] { "C2H6O",                       "C2H5OH",                              "The 17th time this year Rachel has attempted to solve an issue by escaping it internally..." }
    };

    protected float[][] scoreDeltas = {
        new float[] {  0.12f, -0.08f,  0.15f },
        new float[] {  0.15f, -0.10f, -0.15f },
        new float[] {  0.10f, -0.05f,  0.12f },
        new float[] {  0.05f,  0.10f,  0.15f },
        new float[] {  0.05f, -0.05f,  0.18f }
    };

    // -------------------------------------------------------

    private int  currentIndex = 0;
    private bool testFinished = false;

    // -------------------------------------------------------

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        if (endingText  != null) endingText.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(true);

        button1.onClick.AddListener(() => OnChoice(0));
        button2.onClick.AddListener(() => OnChoice(1));
        button3.onClick.AddListener(() => OnChoice(2));

        LoadQuestion(currentIndex);
    }

    // -------------------------------------------------------

    void LoadQuestion(int index)
    {
        if (index >= questions.Length)
        {
            FinishTest();
            return;
        }

        if (dialogueText != null) dialogueText.text = questions[index];
        if (btn1Text     != null) btn1Text.text     = answers[index][0];
        if (btn2Text     != null) btn2Text.text     = answers[index][1];
        if (btn3Text     != null) btn3Text.text     = answers[index][2];
    }

    void OnChoice(int choiceIndex)
    {
        if (testFinished) return;

        uiManager.UpdateScore(scoreDeltas[currentIndex][choiceIndex]);

        currentIndex++;
        LoadQuestion(currentIndex);
    }

    // -------------------------------------------------------

    void FinishTest()
    {
        testFinished = true;
        float score  = uiManager.GetScore();

        if (score >= 0.60f)
        {
            StartCoroutine(ReturnToShip());
        }
        else
        {
            StartCoroutine(LoopTrapRoutine());
        }
    }

    private IEnumerator ReturnToShip()
    {
        if (choicePanel  != null) choicePanel.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);
        if (uiManager.percentageText != null)
            uiManager.percentageText.gameObject.SetActive(false);

        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);
            endingText.text  = "AXIOM: Classification — VERIFIED.\n\nYou are released. For now.";
            endingText.color = new Color(0.2f, 1f, 0.8f);
        }

        yield return new WaitForSeconds(1.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        SceneManager.LoadScene(returnScene); // ✅ Uses the field instead of hardcoded name
    }

    private IEnumerator LoopTrapRoutine()
    {
        if (choicePanel != null) choicePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "AXIOM DETECTED INSUFFICIENT HUMANITY.\nRESTARTING PROTOCOL...";

        yield return new WaitForSeconds(2.0f);

        RestartTest();
    }

    // -------------------------------------------------------

    public void RestartTest()
    {
        currentIndex = 0;
        testFinished = false;
        uiManager.SetScore(0.5f);

        if (uiManager.percentageText != null)
            uiManager.percentageText.gameObject.SetActive(true);

        if (endingText   != null) endingText.gameObject.SetActive(false);
        if (choicePanel  != null) choicePanel.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        LoadQuestion(0);
    }
}
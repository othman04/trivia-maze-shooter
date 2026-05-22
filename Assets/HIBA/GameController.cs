using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public UIManager uiManager;

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

    private string[] questions = {
        "A CAPTCHA asks if you're human.",
        "Which is more human?",
        "Why do people keep embarrassing memories forever?",
        "Why do humans maintain friendships?",
        "Rachel has chosen to remain alone at home for the evening and drink two litres of red wine.\n\nPlease identify the correct chemical formula for consumable alcohol. Is it:"
    };

    private string[][] answers = {
        new string[] { "Feel weirdly offended",      "Complete the task accurately",        "Begin existential crisis" },
        new string[] { "Making mistakes repeatedly",  "Being correct all the time",          "Eliminating inefficiency completely" },
        new string[] { "The brain enjoys torture",    "Emotional memory has high retention", "Permanent cringe archive initialized" },
        new string[] { "Social cooperation",          "Emotional support",                   "To send each other memes instead of discussing the terrifying reality of existence." },
        new string[] { "C2H6O",                       "C2H5OH",                              "The 17th time this year Rachel has attempted to solve an issue by escaping it internally..." }
    };

    private float[][] scoreDeltas = {
        // Q1: CAPTCHA — offended (+12%), accurate (-8%), crisis (+15%)
        new float[] {  0.12f, -0.08f,  0.15f },

        // Q2: More human — mistakes (+15%), correct (-10%), eliminate (-15%)
        new float[] {  0.15f, -0.10f, -0.15f },

        // Q3: Embarrassing memories — torture (+10%), retention (-5%), cringe (+12%)
        new float[] {  0.10f, -0.05f,  0.12f },

        // Q4: Friendships — cooperation (+5%), support (+10%), memes (+15%)
        new float[] {  0.05f,  0.10f,  0.15f },

        // Q5: Rachel — C2H6O (+5%), C2H5OH (-5%), Rachel observation (+18%)
        new float[] {  0.05f, -0.05f,  0.18f }
    };

    // -------------------------------------------------------

    private int  currentIndex = 0;
    private bool testFinished = false;

    // -------------------------------------------------------

    void Start()
    {
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

        if (choicePanel  != null) choicePanel.SetActive(false);
        if (dialogueText != null) dialogueText.gameObject.SetActive(false);

        if (uiManager.percentageText != null) uiManager.percentageText.gameObject.SetActive(false);

        if (endingText != null)
        {
            endingText.gameObject.SetActive(true);

            if (score >= 0.60f)
            {
                endingText.text  = "AXIOM: Classification — VERIFIED.\n\nYou are released. For now.";
                endingText.color = new Color(0.2f, 1f, 0.8f);
            }
            else if (score >= 0.35f)
            {
                endingText.text  = "AXIOM: Classification — UNCERTAIN.\n\nExtended detention initiated.";
                endingText.color = new Color(1f, 0.8f, 0.1f);
            }
            else
            {
                endingText.text  = "AXIOM: Classification — SYNTHETIC.\n\nProcess initiated.";
                endingText.color = new Color(1f, 0.2f, 0.2f);
            }
        }
    }

    // -------------------------------------------------------

    public void RestartTest()
    {
        currentIndex = 0;
        testFinished = false;
        uiManager.SetScore(0.5f);

        if (uiManager.percentageText != null) uiManager.percentageText.gameObject.SetActive(true);

        if (endingText   != null) endingText.gameObject.SetActive(false);
        if (choicePanel  != null) choicePanel.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);

        LoadQuestion(0);
    }
}
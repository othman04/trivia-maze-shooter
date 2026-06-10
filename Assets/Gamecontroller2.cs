using UnityEngine;

public class GameControllerScene2 : GameController
{
    void Awake()
    {
        // Trimmed down to micro-sentences for maximum speed
        questions = new string[] {
            "Crying over a fake character in a book. Calculate the energy efficiency:",
            "Tripping over flat air in public. Correct physical recovery loop?",
            "Eating a brick of cheese over the sink at 1 AM. Identify catalyst:",
            "Unexpected doorbell rings while you're in pajamas. Immediate reflex:",
            "Looking for a street sign while driving. Why turn down the radio?"
        };

        answers = new string[][] {
            new string[] { "0% (Wasted energy/damp blanket).", "100% (Main character behavior).", "Standard ocular salt purge." },
            new string[] { "Fix center of gravity.", "Pretend you started jogging on purpose.", "Log sidewalk friction data." },
            new string[] { "Critical calcium drop.", "Had a mildly annoying day.", "Pre-sleep enzyme optimization." },
            new string[] { "Open it with a smile.", "Drop to the floor. Hide.", "Run property registry scan." },
            new string[] { "Adjusts eye focal length.", "Lower volume helps eyes see better.", "Accelerates spatial rendering." }
        };

        scoreDeltas = new float[][] {
            new float[] { -0.05f,  0.15f, -0.05f }, // Q1
            new float[] { -0.10f,  0.18f, -0.05f }, // Q2
            new float[] { -0.05f,  0.15f, -0.10f }, // Q3
            new float[] { -0.10f,  0.18f, -0.05f }, // Q4
            new float[] { -0.05f,  0.15f, -0.05f }  // Q5
        };
    }
}
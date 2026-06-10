using UnityEngine;

public class GameControllerScene3 : GameController
{
    void Awake()
    {
        // 5 Short, snappy, clinical-vs-absurd questions
        questions = new string[] {
            "Two ex-best friends pass in a hallway without eye contact. What is the distance between them?",
            "Your brain deletes useful high school math but remembers an awkward comment from 2018. Why?",
            "You are completely safe and staring at a pretty sunset. Why are you suddenly crying?",
            "Someone who was mean to you years ago stubbed their toe today. How do you feel?",
            "You are staring into a completely empty fridge for the third time in ten minutes. Why?"
        };

        answers = new string[][] {
            new string[] { "Exactly 1.2 meters.", "An infinite black hole of awkwardness.", "Zero (atomic physics)." },
            new string[] { "Math lacks survival value.", "To violently humble you at 2 AM.", "Standard carbon-hardware glitch." },
            new string[] { "Airborne eye irritant.", "System emotional purge required.", "Fleeting nature of existence." },
            new string[] { "Indifferent.", "Deep, shameful cosmic satisfaction.", "Concerned for their productivity." },
            new string[] { "Checking power grid stability.", "Expecting new food to magically spawn.", "Lowering standards to eat raw cheese." }
        };

        scoreDeltas = new float[][] {
            new float[] { -0.10f,  0.18f, -0.05f }, // Q1 deltas
            new float[] { -0.05f,  0.15f, -0.05f }, // Q2 deltas
            new float[] { -0.05f,  0.12f,  0.15f }, // Q3 deltas
            new float[] { -0.10f,  0.18f, -0.05f }, // Q4 deltas
            new float[] { -0.05f,  0.15f,  0.10f }  // Q5 deltas
        };
    }
}
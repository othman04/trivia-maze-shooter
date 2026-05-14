using UnityEngine;
using UnityEngine.UI; // Obligatoire pour manipuler l'UI

public class SlidingDoorManager : MonoBehaviour
{
    [Header("Portes")]
    public Transform doorLeft;
    public Transform doorRight;

    [Header("Interface Utilisateur")]
    public GameObject promptE; // Glissez votre objet texte ici

    [Header("Réglages")]
    public float slideDistance = 1.8f; 
    public float speed = 5f;

    private Vector3 initialPosLeft;
    private Vector3 initialPosRight;
    private Vector3 targetPosLeft;
    private Vector3 targetPosRight;

    private bool isPlayerInside = false;
    private bool isOpen = false;

    void Start()
    {
        initialPosLeft = doorLeft.localPosition;
        initialPosRight = doorRight.localPosition;
        targetPosLeft = initialPosLeft;
        targetPosRight = initialPosRight;

        // On s'assure que le texte est caché au lancement
        if(promptE != null) promptE.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoors();
        }

        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, targetPosLeft, Time.deltaTime * speed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, targetPosRight, Time.deltaTime * speed);
    }

    void ToggleDoors()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            targetPosLeft = initialPosLeft + new Vector3(0, 0, slideDistance);
            targetPosRight = initialPosRight + new Vector3(0, 0, -slideDistance);
        }
        else
        {
            targetPosLeft = initialPosLeft;
            targetPosRight = initialPosRight;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerInside = true;
            if(promptE != null) promptE.SetActive(true); // Affiche le texte
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if(promptE != null) promptE.SetActive(false); // Cache le texte
            if (isOpen) ToggleDoors(); 
        }
    }
}
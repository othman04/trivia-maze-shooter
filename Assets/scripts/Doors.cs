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

    [Header("Audio (Ajouté)")]
    public AudioClip slidingSound;      // Glisse ton fichier son ici
    [Range(0f, 1f)] public float volume = 0.5f;

    private Vector3 initialPosLeft;
    private Vector3 initialPosRight;
    private Vector3 targetPosLeft;
    private Vector3 targetPosRight;

    private bool isPlayerInside = false;
    private bool isOpen = false;

    // Variables pour la gestion audio interne
    private AudioSource audioSource;
    private bool wasMoving = false;

    void Start()
    {
        initialPosLeft = doorLeft.localPosition;
        initialPosRight = doorRight.localPosition;
        targetPosLeft = initialPosLeft;
        targetPosRight = initialPosRight;

        // On s'assure que le texte est caché au lancement
        if(promptE != null) promptE.SetActive(false);

        // --- CONFIGURATION AUDIO AUTOMATIQUE ---
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f; // Son en 3D (localisé sur la porte)
        audioSource.volume = volume;
    }

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoors();
        }

        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, targetPosLeft, Time.deltaTime * speed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, targetPosRight, Time.deltaTime * speed);

        // --- GESTION DU SON EN TEMPS RÉEL ---
        // On vérifie si les portes bougent encore (si la distance vers la cible est grande)
        bool isMovingNow = Vector3.Distance(doorLeft.localPosition, targetPosLeft) > 0.01f;

        if (isMovingNow && !wasMoving)
        {
            // La porte commence tout juste à glisser
            PlayDoorSound();
        }
        else if (!isMovingNow && wasMoving)
        {
            // La porte vient d'atteindre sa destination (ouverte ou fermée complètement)
            StopDoorSound();
        }

        wasMoving = isMovingNow;
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

    // --- FONCTIONS AUDIO COUTUMIERES ---
    void PlayDoorSound()
    {
        if (slidingSound != null && audioSource != null)
        {
            audioSource.clip = slidingSound;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    void StopDoorSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
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
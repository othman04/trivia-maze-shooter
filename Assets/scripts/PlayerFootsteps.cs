using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footsteps Audio")]
    public AudioClip[] footstepSounds;
    public float timeBetweenSteps = 0.5f; 

    [Header("Jump Audio")]
    public AudioClip jumpSound;

    private AudioSource footstepAudioSource; // Canal 1 : Dédié aux pas
    private AudioSource jumpAudioSource;     // Canal 2 : Dédié au saut
    
    private CharacterController controller;
    private float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // --- CONFIGURATION AUTOMATIQUE DES DEUX CANAUX ---
        // On crée le premier canal pour les pas
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.loop = false;

        // On crée le deuxième canal pour le saut
        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.loop = false;
        
        // Optionnel : Si vous aviez déjà un composant AudioSource sur l'objet, 
        // on copie son volume pour que ce soit propre
        AudioSource originalSource = GetComponent<AudioSource>();
        if (originalSource != null && originalSource != footstepAudioSource && originalSource != jumpAudioSource)
        {
            footstepAudioSource.volume = originalSource.volume;
            jumpAudioSource.volume = originalSource.volume;
        }
    }

    void Update()
    {
        if (controller == null || footstepAudioSource == null) return;

        // 1. Calcul de la vitesse au sol (X et Z uniquement)
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // Seuil de détection de marche
        bool isMovingReal = currentSpeed > 1.2f;

        if (controller.isGrounded && isMovingReal)
        {
            stepTimer += Time.deltaTime;

            float currentStepDelay = timeBetweenSteps;
            
            // Si le joueur court, on accélère le rythme
            if (currentSpeed > 5.0f) 
            {
                currentStepDelay = timeBetweenSteps * 0.6f; 
            }

            if (stepTimer >= currentStepDelay)
            {
                PlayFootstepSound();
                stepTimer = 0f; 
            }
        }
        else
        {
            stepTimer = 0f;
            
            // On coupe UNIQUEMENT le canal des pas quand on s'arrête ou qu'on saute.
            // Le canal du saut, lui, reste libre de jouer !
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop(); 
            }
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0 || footstepAudioSource == null) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        footstepAudioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    // Cette fonction est appelée par le script FirstPersonController d'Unity au moment du saut
    public void PlayJumpSound()
    {
        if (jumpSound != null && jumpAudioSource != null)
        {
            // On utilise le canal dédié au saut, totalement indépendant de la marche !
            jumpAudioSource.PlayOneShot(jumpSound);
        }
    }
}
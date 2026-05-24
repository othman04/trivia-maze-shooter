using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footsteps Audio")]
    public AudioClip[] footstepSounds;
    public float timeBetweenSteps = 0.5f;

    [Header("Jump & Land Audio")]
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("Water Audio")]
    public AudioClip waterAmbientSound;
    public AudioClip waterJumpSound;
    public AudioClip waterLandSound;

    [Header("Water Threshold")]
    public float waterYThreshold = 0f;

    private AudioSource footstepAudioSource;
    private AudioSource jumpAudioSource;
    private AudioSource waterAudioSource;
    private CharacterController controller;
    private float stepTimer = 0f;
    private bool wasGrounded = true;
    private bool jumpedFromWater = false;

    bool IsInWater()
    {
        float feetY = transform.position.y - (controller.height / 2f);
        return feetY <= waterYThreshold;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.loop = false;

        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.loop = false;

        waterAudioSource = gameObject.AddComponent<AudioSource>();
        waterAudioSource.playOnAwake = false;
        waterAudioSource.loop = true;
        waterAudioSource.clip = waterAmbientSound;

        AudioSource originalSource = GetComponent<AudioSource>();
        if (originalSource != null && originalSource != footstepAudioSource && originalSource != jumpAudioSource && originalSource != waterAudioSource)
        {
            footstepAudioSource.volume = originalSource.volume;
            jumpAudioSource.volume = originalSource.volume;
            waterAudioSource.volume = originalSource.volume;
        }
    }

    void Update()
    {
        if (controller == null || footstepAudioSource == null) return;

        // Water ambient loop
        if (IsInWater())
        {
            if (!waterAudioSource.isPlaying)
                waterAudioSource.Play();
        }
        else
        {
            if (waterAudioSource.isPlaying)
                waterAudioSource.Stop();
        }

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;
        bool isMovingReal = currentSpeed > 1.2f;

        // Landing detection
        if (!wasGrounded && controller.isGrounded)
        {
            if (jumpedFromWater || IsInWater())
            {
                if (waterLandSound != null)
                    jumpAudioSource.PlayOneShot(waterLandSound);
            }
            else
            {
                if (landSound != null)
                    jumpAudioSource.PlayOneShot(landSound);
            }
            jumpedFromWater = false;
        }

        wasGrounded = controller.isGrounded;

        // Footsteps on land only
        if (controller.isGrounded && isMovingReal && !IsInWater())
        {
            stepTimer += Time.deltaTime;

            float currentStepDelay = timeBetweenSteps;
            if (currentSpeed > 5.0f)
                currentStepDelay = timeBetweenSteps * 0.6f;

            if (stepTimer >= currentStepDelay)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
            if (footstepAudioSource.isPlaying && !IsInWater())
                footstepAudioSource.Stop();
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepAudioSource == null || footstepSounds.Length == 0) return;

        int i = Random.Range(0, footstepSounds.Length);
        footstepAudioSource.clip = footstepSounds[i];
        footstepAudioSource.Play();
    }

    public void PlayJumpSound()
    {
        jumpedFromWater = IsInWater();

        if (jumpedFromWater && waterJumpSound != null)
            jumpAudioSource.PlayOneShot(waterJumpSound);
        else if (jumpSound != null)
            jumpAudioSource.PlayOneShot(jumpSound);
    }
}
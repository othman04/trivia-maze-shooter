using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

#pragma warning disable 618, 649
namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        // ── Movement (from Script 1) ───────────────────────────────────
        [Header("Movement")]
        [SerializeField] private bool  m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;

        // ── FOV Kick (from Script 1) ───────────────────────────────────
        [Header("FOV Kick")]
        [SerializeField] private bool    m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();

        // ── Head Bob ───────────────────────────────────────────────────
        [Header("Head Bob")]
        [SerializeField] private bool             m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob  m_JumpBob = new LerpControlledBob();

        // ── Footstep Settings ──────────────────────────────────────────
        [Header("Footstep Settings")]
        [Tooltip("Distance walked between each footstep sound.")]
        [SerializeField] private float m_StepInterval = 5f;

        // ── Normal Sounds ──────────────────────────────────────────────
        [Header("Normal Sounds")]
        [SerializeField] private AudioClip[] m_FootstepSounds;
        [SerializeField] [Range(0f, 1f)] private float m_FootstepVolume = 1f;
        [SerializeField] private AudioClip m_JumpSound;
        [SerializeField] [Range(0f, 1f)] private float m_JumpVolume = 1f;
        [SerializeField] private AudioClip m_LandSound;
        [SerializeField] [Range(0f, 1f)] private float m_LandVolume = 1f;

        // ── Water Sounds ───────────────────────────────────────────────
        [Header("Water Sounds")]
        [SerializeField] private AudioClip[] m_WaterFootstepSounds;
        [SerializeField] [Range(0f, 1f)] private float m_WaterFootstepVolume = 1f;
        [SerializeField] private AudioClip m_WaterLandSound;
        [SerializeField] [Range(0f, 1f)] private float m_WaterLandVolume = 1f;

        // ── Swimming (acceleration in water on Space) ──────────────────
        [Header("Swimming")]
        [Tooltip("Speed boost applied forward when Space is pressed in water.")]
        [SerializeField] private float m_SwimAcceleration = 5f;
        [Tooltip("Maximum speed the swim boost can reach.")]
        [SerializeField] private float m_SwimMaxSpeed = 10f;
        [Tooltip("How quickly the swim boost decays each second.")]
        [SerializeField] private float m_SwimDrag = 3f;
        [SerializeField] private AudioClip m_SwimmingSound;
        [SerializeField] [Range(0f, 1f)] private float m_SwimmingVolume = 1f;

        // ── Water Zone ─────────────────────────────────────────────────
        [Header("Water Zone")]
        [Tooltip("Player feet AT or BELOW this Y value = in water.")]
        [SerializeField] private float m_WaterYThreshold = 0f;

        // ── Intro Monologue ────────────────────────────────────────────
        [Header("Intro Monologue")]
        [Tooltip("Plays 1 second after the game starts.")]
        [SerializeField] private AudioClip m_IntroClipA;
        [SerializeField] [Range(0f, 1f)] private float m_IntroClipAVolume = 1f;
        [Tooltip("Plays 5 seconds after Clip A finishes.")]
        [SerializeField] private AudioClip m_IntroClipB;
        [SerializeField] [Range(0f, 1f)] private float m_IntroClipBVolume = 1f;

        // ── Private State ──────────────────────────────────────────────
        private Camera              m_Camera;
        private CharacterController m_CharacterController;
        private AudioSource         m_AudioSource;
        private AudioSource         m_IntroAudioSource;

        private Vector2        m_Input;
        private Vector3        m_MoveDir = Vector3.zero;
        private CollisionFlags m_CollisionFlags;

        private bool  m_Jump;
        private bool  m_Jumping;
        private bool  m_PreviouslyGrounded;
        private bool  m_InWater;

        // Accumulated swim boost velocity (forward axis only, world space)
        private float m_SwimBoost;

        private Vector3 m_OriginalCameraPosition;
        private float   m_StepCycle;
        private float   m_NextStep;

        // ── Start ──────────────────────────────────────────────────────
        private void Start()
        {
            m_CharacterController    = GetComponent<CharacterController>();
            m_AudioSource            = GetComponent<AudioSource>();
            m_Camera                 = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;

            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);

            m_StepCycle = 0f;
            m_NextStep  = m_StepInterval;
            m_Jumping   = false;
            m_SwimBoost = 0f;

            m_MouseLook.Init(transform, m_Camera.transform);

            m_IntroAudioSource             = gameObject.AddComponent<AudioSource>();
            m_IntroAudioSource.playOnAwake = false;
            m_IntroAudioSource.spatialBlend = 0f;

            StartCoroutine(PlayIntroSequence());
        }

        // ── Update ─────────────────────────────────────────────────────
        private void Update()
        {
            RotateView();
            CheckWaterByHeight();

            if (!m_Jump)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

            // Landing detection
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping   = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
                m_MoveDir.y = 0f;

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        // ── Fixed Update ───────────────────────────────────────────────
        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            RaycastHit hitInfo;
            Physics.SphereCast(
                transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_InWater)
            {
                // ── WATER: Space = swim acceleration, never jump ────────
                if (m_Jump)
                {
                    // Accelerate forward
                    m_SwimBoost = Mathf.Min(m_SwimBoost + m_SwimAcceleration, m_SwimMaxSpeed);
                    PlaySwimmingSound();
                    m_Jump = false;   // consume input — no jump
                }

                // Apply & decay swim boost along world-forward
                m_SwimBoost = Mathf.MoveTowards(m_SwimBoost, 0f, m_SwimDrag * Time.fixedDeltaTime);
                Vector3 swimVelocity = transform.forward * m_SwimBoost;
                m_MoveDir.x += swimVelocity.x;
                m_MoveDir.z += swimVelocity.z;

                // Still apply gravity so the player stays on the water-floor surface
                if (!m_CharacterController.isGrounded)
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                else
                    m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                // ── LAND: normal jump logic ────────────────────────────
                m_SwimBoost = 0f;   // clear any leftover boost on exit

                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump    = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
            m_MouseLook.UpdateCursorLock();
        }

        // ── Water Detection ────────────────────────────────────────────
        private void CheckWaterByHeight()
        {
            float feetY    = transform.position.y - (m_CharacterController.height / 2f);
            bool  wasInWater = m_InWater;
            m_InWater      = feetY <= m_WaterYThreshold;

            if (wasInWater && !m_InWater)
            {
                m_AudioSource.Stop();
                m_SwimBoost = 0f;
                m_StepCycle = 0f;
                m_NextStep  = m_StepInterval;
            }
        }

        // ── Intro Sequence ─────────────────────────────────────────────
        private IEnumerator PlayIntroSequence()
        {
            yield return new WaitForSeconds(1f);

            if (m_IntroClipA != null)
            {
                m_IntroAudioSource.volume = m_IntroClipAVolume;
                m_IntroAudioSource.clip   = m_IntroClipA;
                m_IntroAudioSource.Play();
                yield return new WaitForSeconds(m_IntroClipA.length + 5f);
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }

            if (m_IntroClipB != null)
            {
                m_IntroAudioSource.volume = m_IntroClipBVolume;
                m_IntroAudioSource.clip   = m_IntroClipB;
                m_IntroAudioSource.Play();
            }
        }

        // ── Footstep Cycle ─────────────────────────────────────────────
        private void ProgressStepCycle(float speed)
        {
            if (!m_CharacterController.isGrounded ||
                m_CharacterController.velocity.sqrMagnitude <= 0f ||
                (m_Input.x == 0f && m_Input.y == 0f))
            {
                m_StepCycle = 0f;
                m_NextStep  = m_StepInterval;
                return;
            }

            m_StepCycle += (m_CharacterController.velocity.magnitude +
                            (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;

            if (m_StepCycle < m_NextStep) return;

            m_NextStep = m_StepCycle + m_StepInterval;
            PlayFootstep();
        }

        // ── Sound Helpers ──────────────────────────────────────────────
        private void PlayFootstep()
        {
            if (!m_CharacterController.isGrounded) return;

            m_AudioSource.Stop();

            if (m_InWater)
            {
                if (m_WaterFootstepSounds == null || m_WaterFootstepSounds.Length == 0) return;
                AudioClip clip = m_WaterFootstepSounds[Random.Range(0, m_WaterFootstepSounds.Length)];
                if (clip != null) m_AudioSource.PlayOneShot(clip, m_WaterFootstepVolume);
            }
            else
            {
                if (m_FootstepSounds == null || m_FootstepSounds.Length == 0) return;
                // Pick & rotate to avoid repeats (Script 1 behaviour)
                int n = Random.Range(1, m_FootstepSounds.Length);
                m_AudioSource.clip = m_FootstepSounds[n];
                m_AudioSource.PlayOneShot(m_AudioSource.clip, m_FootstepVolume);
                m_FootstepSounds[n] = m_FootstepSounds[0];
                m_FootstepSounds[0] = m_AudioSource.clip;
            }
        }

        private void PlayJumpSound()
        {
            if (m_JumpSound != null)
                m_AudioSource.PlayOneShot(m_JumpSound, m_JumpVolume);
        }

        private void PlayLandingSound()
        {
            AudioClip clip   = m_InWater ? m_WaterLandSound  : m_LandSound;
            float     volume = m_InWater ? m_WaterLandVolume : m_LandVolume;
            if (clip != null)
            {
                m_AudioSource.Stop();
                m_AudioSource.PlayOneShot(clip, volume);
                m_StepCycle = 0f;
                m_NextStep  = m_StepInterval;
            }
        }

        private void PlaySwimmingSound()
        {
            if (m_SwimmingSound != null)
                m_AudioSource.PlayOneShot(m_SwimmingSound, m_SwimmingVolume);
        }

        // ── Input (walk / run from Script 1) ───────────────────────────
        private void GetInput(out float speed)
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical   = CrossPlatformInputManager.GetAxis("Vertical");

            bool wasWalking = m_IsWalking;

#if !MOBILE_INPUT
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            speed   = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            if (m_Input.sqrMagnitude > 1f)
                m_Input.Normalize();

            // FOV kick on walk <-> run transition
            if (m_IsWalking != wasWalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0f)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        // ── View ───────────────────────────────────────────────────────
        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        // ── Camera Bob ─────────────────────────────────────────────────
        private void UpdateCameraPosition(float speed)
        {
            if (!m_UseHeadBob) return;

            Vector3 newPos;
            if (m_CharacterController.velocity.magnitude > 0f && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                        (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newPos   = m_Camera.transform.localPosition;
                newPos.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newPos   = m_Camera.transform.localPosition;
                newPos.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newPos;
        }

        // ── Physics Push ───────────────────────────────────────────────
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (m_CollisionFlags == CollisionFlags.Below) return;
            if (body == null || body.isKinematic) return;
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}

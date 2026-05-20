using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection & Combat")]
    public float detectionRange = 20f;   // Distance de détection acceptable
    public float shootingRange = 15f;    // Distance à laquelle il commence à tirer
    public float stopDistance = 8f;      // Il s'arrête à 8 mètres de vous (ne vous colle pas)
    public float fireRate = 0.5f;        // Il tire toutes les 0.5 secondes

    [Header("Patrol Settings")]
    public float patrolRadius = 30f;     // Marge très grande de déplacement aléatoire
    public float patrolWaitTime = 3f;    // Temps d'attente à chaque point de patrouille

    [Header("References")]
    public GameObject enemyBulletPrefab; // Prefab de la balle de l'ennemi
    public Transform gunMuzzle;          // Le bout du fusil de l'ennemi

    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;
    
    private float nextTimeToFire = 0f;
    private float patrolTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.stoppingDistance = stopDistance; // Force l'agent à s'arrêter à distance
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Choisir un premier point de patrouille au démarrage
        GoToRandomPatrolPoint();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // --- ÉTAPE A : DÉTECTION ET POURSUITE ---
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            agent.SetDestination(playerTransform.position);

            // Toujours faire face au joueur quand il est proche
            LookAtPlayer();

            // --- ÉTAPE B : LOGIQUE DE TIR ---
            if (distanceToPlayer <= shootingRange && Time.time >= nextTimeToFire)
            {
                EnemyShoot();
            }
        }
        // --- ÉTAPE C : PATROUILLE ALÉATOIRE (si le joueur est loin) ---
        else
        {
            if (isChasing)
            {
                // Si le joueur vient de s'enfuir, on réinitialise et on reprend la patrouille
                isChasing = false;
                GoToRandomPatrolPoint();
            }

            // Si l'ennemi est arrivé à son point de patrouille (ou presque)
            if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolWaitTime)
                {
                    GoToRandomPatrolPoint();
                    patrolTimer = 0f;
                }
            }
        }

        // --- ÉTAPE D : ANIMATIONS ---
        if (animator != null && agent != null)
        {
            float currentSpeed = 0f;
            if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
            {
                currentSpeed = agent.speed;
            }
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; // Empêche l'ennemi de se pencher bizarrement en haut ou en bas
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void GoToRandomPatrolPoint()
    {
        // Calcule un point aléatoire dans une grande sphère sur le NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        // Trouve la position valide la plus proche sur la surface bleue du NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void EnemyShoot()
    {
        nextTimeToFire = Time.time + fireRate;

        // Déclenche l'animation de tir si vous en avez une (ex: "Shoot" ou "Attack")
        if (animator != null)
        {
            animator.SetTrigger("Shoot"); 
        }

        if (enemyBulletPrefab != null && gunMuzzle != null)
        {
            // Calcule la direction vers le joueur (visée au niveau du torse pour éviter le sol)
            Vector3 targetPosition = playerTransform.position + Vector3.up * 1f; 
            Vector3 fireDirection = (targetPosition - gunMuzzle.position).normalized;

            // Instancie la balle de l'ennemi
            GameObject bulletGO = Instantiate(enemyBulletPrefab, gunMuzzle.position, Quaternion.LookRotation(fireDirection));
            
            // On récupère le script de balle pour la propulser
            Bullet bulletScript = bulletGO.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Launch(fireDirection);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Zone de détection en Rouge
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Zone de tir en Bleu
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
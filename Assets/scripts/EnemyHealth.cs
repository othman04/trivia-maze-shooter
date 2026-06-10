using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Destruction Settings")]
    [Tooltip("Temps en secondes avant que le corps ne disparaisse après la mort. Mets 0 pour une disparition instantanée.")]
    public float timeBeforeDisappearing = 1.5f; // Ajuste cette valeur dans l'inspecteur !

    private bool isDead = false;
    private Animator animator;
    private NavMeshAgent agent;
    private Collider enemyCollider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"L'ennemi {gameObject.name} a reçu des dégâts. Vie restante : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log($"L'ennemi {gameObject.name} est mort !");

        // 1. On coupe l'IA pour éviter qu'il continue de te poursuivre par terre
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // 2. On coupe le script de combat pour qu'il s'arrête de te tirer dessus
        EnemyAI aiScript = GetComponent<EnemyAI>();
        if (aiScript != null)
        {
            aiScript.enabled = false; 
        }

        // 3. On coupe ses collisions pour que tes balles passent à travers son cadavre
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // 4. On lance l'animation de mort si elle existe
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Assure-toi d'avoir un Trigger nommé "Die" dans ton Animator
        }

        // 5. FIX CORRECTION : Destruction du soldat contrôlée par la variable
        // Si tu mets 0 dans l'inspecteur, il disparaîtra à la milliseconde près.
        // Si tu laisses 1.5f, il attend la fin de son animation puis s'efface proprement.
        Destroy(gameObject, timeBeforeDisappearing);
    }
}
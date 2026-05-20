using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private Animator animator;
    private NavMeshAgent agent;
    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
    }

    // Cette fonction sera appelée par la balle
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return; // Si déjà mort, on ne fait rien

        currentHealth -= damageAmount;
        Debug.Log("L'ennemi a reçu " + damageAmount + " dégâts. Vie restante : " + currentHealth);

        // Optionnel : Déclencher une animation de coup reçu si elle existe
        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("Hit"); // Vérifiez le nom dans votre Animator Synty
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("L'ennemi est mort !");

        // 1. Désactiver les scripts d'IA pour qu'il arrête de poursuivre
        if (agent != null) agent.enabled = false;
        if (enemyAI != null) enemyAI.enabled = false;

        // 2. Désactiver le collider pour que les balles passent à travers son cadavre
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider != null) collider.enabled = false;

        // 3. Déclencher l'animation de mort
        if (animator != null)
        {
            // Note : Dans les packs Synty, le paramètre de mort est souvent un Trigger nommé "Death" ou un booléen "isDead"
            animator.SetTrigger("Death"); 
        }

        // 4. (Optionnel) Détruire le corps après 5 secondes pour alléger le jeu
        Destroy(gameObject, 5f);
    }
}
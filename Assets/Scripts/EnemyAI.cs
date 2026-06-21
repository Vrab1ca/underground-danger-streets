using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum State { Idle, Chase, Attack, Dead }

    public State state = State.Idle;

    [Header("Target")]
    public Transform player;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("AI Settings")]
    public float detectionRange = 18f;
    public float attackRange = 1.8f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    [Header("Animation")]
    public Animator animator;

    private NavMeshAgent agent;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }
    }

    private void Update()
    {
        if (state == State.Dead || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
            state = State.Attack;
        else if (distance <= detectionRange)
            state = State.Chase;
        else
            state = State.Idle;

        if (state == State.Idle)
        {
            agent.isStopped = true;

            if (animator != null)
                animator.SetBool("Moving", false);
        }
        else if (state == State.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator != null)
                animator.SetBool("Moving", true);
        }
        else if (state == State.Attack)
        {
            agent.isStopped = true;

            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (animator != null)
                animator.SetBool("Moving", false);

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;

                if (animator != null)
                    animator.SetTrigger("Attack");

                IDamageable dmg = player.GetComponentInParent<IDamageable>();

                if (dmg != null)
                    dmg.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (state == State.Dead)
            return;

        currentHealth -= amount;

        Debug.Log(gameObject.name + " took damage: " + amount + " HP left: " + currentHealth);

        state = State.Chase;

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        state = State.Dead;

        if (agent != null)
            agent.isStopped = true;

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject, 3f);
    }
}
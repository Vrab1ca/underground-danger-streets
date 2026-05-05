using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum State { Idle, Chase, Attack, Dead }

    public State state = State.Idle;
    public Transform player;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float detectionRange = 18f;
    public float attackRange = 1.8f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public Animator animator;

    private NavMeshAgent agent;
    private float nextAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (state == State.Dead || player == null) return;

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
            if (animator != null) animator.SetBool("Moving", false);
        }
        else if (state == State.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            if (animator != null) animator.SetBool("Moving", true);
        }
        else if (state == State.Attack)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                if (animator != null) animator.SetTrigger("Attack");

                IDamageable dmg = player.GetComponentInParent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (state == State.Dead) return;

        currentHealth -= amount;
        state = State.Chase;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        state = State.Dead;
        agent.isStopped = true;
        if (animator != null) animator.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }
}

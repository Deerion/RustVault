using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : LivingEntity
{
    public enum EnemyState { Patrol, Chase, Attack }

    [Header("Sztuczna Inteligencja")]
    public EnemyState currentState = EnemyState.Patrol;
    public float aggroRange = 15f;
    public float attackRange = 1.2f; // Zmniejszone, by podchodził pod sam nos

    [Header("Patrol (Swobodne chodzenie)")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    [Header("Walka")]
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    [Header("Optymalizacja AI")]
    public float pathUpdateDelay = 0.15f;  // Wróg aktualizuje cel co 0.15 sekundy (zapobiega zacinaniu)
    private float pathUpdateTimer = 0f;

    [Header("Referencje")]
    public NavMeshAgent agent;
    public Animator animator; 
    private Transform playerTransform;

    private void Awake()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        isDead = false;
    }

    protected override void Start()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Update()
    {
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = maxHealth;
        }

        if (isDead || playerTransform == null || agent == null || !agent.isOnNavMesh) return;

        // Ignorujemy wysokość (oś Y). Liczymy odległość tylko "na płasko".
        Vector3 enemyPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosFlat = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

        float distanceToPlayer = Vector3.Distance(enemyPosFlat, playerPosFlat);

        // MASZYNA STANÓW WROGA (FSM)
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= aggroRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        UndergoStateAction();

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void UndergoStateAction()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    public void Patrol()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
                randomDirection += transform.position;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
                {
                    agent.SetDestination(hit.position);
                    patrolTimer = 0f;
                }
            }
        }
    }

    public void ChasePlayer()
    {
        if (!agent.isOnNavMesh) return;

        agent.isStopped = false; // Upewniamy się, że wróg może chodzić

        // Zamiast wyliczać trasę co klatkę, korzystamy z licznika czasu
        pathUpdateTimer += Time.deltaTime;

        if (pathUpdateTimer > pathUpdateDelay)
        {
            agent.SetDestination(playerTransform.position);
            pathUpdateTimer = 0f; // Resetujemy stoper
        }
    }

    public void Attack()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Potwór zadaje obrażenia graczowi!");

            animator.SetTrigger("Attack"); 

            lastAttackTime = Time.time;

            IDamageable target = playerTransform.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(25);
            }
        }
    }

    public override void Die()
    {
        Debug.Log("Potwór został zlikwidowany!");
        base.Die();
    }
}
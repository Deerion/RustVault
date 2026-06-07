using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : LivingEntity
{
    public enum EnemyState { Patrol, Chase, Attack }

    [Header("Sztuczna Inteligencja")]
    public EnemyState currentState = EnemyState.Patrol;
    public float aggroRange = 15f;
    public float attackRange = 1.2f;

    [Header("Patrol (Swobodne chodzenie)")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    [Header("Walka")]
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    [Header("Optymalizacja AI")]
    public float pathUpdateDelay = 0.15f;
    private float pathUpdateTimer = 0f;

    [Header("Referencje")]
    public NavMeshAgent agent;
    public Animator animator;
    private Transform playerTransform;

    [Header("Dźwięki")]
    public AudioClip hitSound;     // Dźwięk, gdy MY uderzamy wroga
    public AudioClip attackSound;  // Dźwięk, gdy wróg uderza nas
    public AudioClip deathSound;   // Dźwięk śmierci wroga (body-fall)
    private AudioSource audioSource;

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

        Vector3 enemyPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosFlat = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);

        float distanceToPlayer = Vector3.Distance(enemyPosFlat, playerPosFlat);

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

        // Pobieramy prędkość
        float currentSpeed = agent.velocity.magnitude;

        // Dodajemy amortyzator (0.1f), który ignoruje chwilowe zacięcia
        animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
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

        agent.isStopped = false;
        pathUpdateTimer += Time.deltaTime;

        if (pathUpdateTimer > pathUpdateDelay)
        {
            agent.SetDestination(playerTransform.position);
            pathUpdateTimer = 0f;
        }
    }

    public void Attack()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

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

    // Nadpisujemy metodę obrywania, żeby dodać dźwięk
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Wykonujemy standardowe odejmowanie HP z LivingEntity

        if (audioSource != null && hitSound != null && !isDead)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    public override void Die()
    {
        Debug.Log("Potwór został zlikwidowany!");

        if (audioSource != null && deathSound != null)
        {
            // Trik z odpięciem dźwięku dla usuwanego obiektu
            audioSource.transform.parent = null;
            audioSource.PlayOneShot(deathSound);
            Destroy(audioSource.gameObject, deathSound.length);
        }

        base.Die();
    }

    private IEnumerator DealDamageWithDelay(float delay)
    {
        // 1. Czekamy ułamek sekundy (aż pięść z animacji zbliży się do gracza)
        yield return new WaitForSeconds(delay);

        if (playerTransform == null || isDead) yield break;

        // 2. Ponownie liczymy dystans (po tym, jak daliśmy graczowi czas na ucieczkę)
        Vector3 enemyPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosFlat = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        float currentDistance = Vector3.Distance(enemyPosFlat, playerPosFlat);

        // 3. Dodajemy mały margines błędu do zasięgu (np. 0.5f), żeby atak był sprawiedliwy
        if (currentDistance <= attackRange + 0.5f)
        {
            Debug.Log("Cios trafił gracza!");
            IDamageable target = playerTransform.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(25);
            }
        }
        else
        {
            Debug.Log("Pudło! Gracz zdążył odskoczyć do tyłu.");
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : LivingEntity
{
    public enum EnemyState { Patrol, Chase, Attack }

    [Header("Sztuczna Inteligencja")]
    public EnemyState currentState = EnemyState.Patrol;
    public float aggroRange = 7f;
    public float attackRange = 1.5f;

    [Header("Referencje")]
    public NavMeshAgent agent;
    private Transform playerTransform;

    // Używamy Awake zamiast Start do ustawienia życia, żeby od razu było 100/100 HP!
    private void Awake()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        isDead = false;
    }

    protected override void Start()
    {
        // Ignorujemy domyślny Start z LivingEntity, bo zdrowie ustawiliśmy już w Awake

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
        // Zabezpieczenie: jeśli w edytorze dalej masz 0 HP, wymuszamy uleczenie potwora
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = maxHealth;
        }

        if (isDead || playerTransform == null || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

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
        if (agent.isOnNavMesh) agent.isStopped = true;
    }

    public void ChasePlayer()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
    }

    public void Attack()
    {
        if (agent.isOnNavMesh) agent.isStopped = true;
        Debug.Log("Potwór atakuje gracza!");
    }

    public override void Die()
    {
        Debug.Log("Potwór EnemyAI został pokonany przez gracza!");
        base.Die();
    }
}
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class IAPatrol : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform[] patrolPoints;
    private Transform playerTarget;
    private Vector3 investigationTargetPosition; // NOVO: Guarda a posição para investigar

    [Header("Parâmetros de Patrulha")]
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Parâmetros de Perseguição")]
    [SerializeField] private float huntingSpeed = 6f;
    [SerializeField] private float waitTimeAfterHunt = 3f;

    [Header("Parâmetros de Ataque")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 2.5f;
    [SerializeField] private float pauseAfterAttack = 1.5f;

    // NOVO: Adicionado estado de Investigação
    private enum PatrolState
    {
        Patrolling,
        Hunting,
        WaitingAfterHunt,
        Investigating
    }
    [SerializeField] private PatrolState currentState;

    private NavMeshAgent navMeshAgent;
    private int lastPatrolIndex = -1;
    private bool isAgentActive = true;
    private float lastAttackTime = -99f;

    // --- MÉTODOS PRINCIPAIS DA UNITY ---

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("Nenhum ponto de patrulha foi definido!", this);
            isAgentActive = false;
            return;
        }
        currentState = PatrolState.Patrolling;
        navMeshAgent.speed = patrolSpeed;
        MoveToNextPatrolPoint();
    }

    void Update()
    {
        if (!isAgentActive) return;

        // Lógica de cada estado
        if (currentState == PatrolState.Patrolling)
        {
            // Se chegou ao ponto de patrulha, espera
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                StartCoroutine(WaitAtPoint());
            }
        }
        else if (currentState == PatrolState.Hunting)
        {
            // Persegue o jogador e ataca se estiver perto
            if (playerTarget != null)
            {
                navMeshAgent.destination = playerTarget.position;
                if (Vector3.Distance(transform.position, playerTarget.position) <= attackDistance)
                {
                    AttackPlayer();
                }
            }
        }
        // NOVO: Lógica para o estado de Investigação
        else if (currentState == PatrolState.Investigating)
        {
            // Se chegou ao ponto de investigação, espera e depois volta a patrulhar
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                StartCoroutine(FinishInvestigation());
            }
        }
    }

    // --- NOVO: MÉTODO PÚBLICO PARA CHAMAR O INIMIGO ---

    /// <summary>
    /// Ordena que o inimigo vá para uma localização específica para investigar.
    /// Esta função pode ser chamada por outros scripts.
    /// </summary>
    /// <param name="targetLocation">O Transform do local para onde o inimigo deve ir.</param>
    public void CallToLocation(Transform targetLocation)
    {
        // A caça ao jogador sempre tem prioridade máxima.
        if (currentState == PatrolState.Hunting)
        {
            Debug.Log("Inimigo está caçando e ignorou o chamado.");
            return;
        }

        Debug.Log($"<color=lightblue>Inimigo chamado para investigar a posição {targetLocation.position}</color>");

        StopAllCoroutines();
        isAgentActive = true;
        navMeshAgent.isStopped = false;

        currentState = PatrolState.Investigating;
        investigationTargetPosition = targetLocation.position;
        navMeshAgent.speed = patrolSpeed; // Usa a velocidade de patrulha para investigar
        navMeshAgent.destination = investigationTargetPosition;
    }

    // --- LÓGICA DE ESTADOS E AÇÕES ---

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            PlayerMoviment player = playerTarget.GetComponent<PlayerMoviment>();
            if (player != null)
            {
                player.TakeDamage(1);
                StartCoroutine(PauseAfterAttack());
            }
        }
    }

    private IEnumerator PauseAfterAttack()
    {
        navMeshAgent.isStopped = true;
        Debug.Log("Inimigo atacou e está em pausa.");
        yield return new WaitForSeconds(pauseAfterAttack);
        navMeshAgent.isStopped = false;
    }

    private IEnumerator WaitAtPoint()
    {
        isAgentActive = false;
        yield return new WaitForSeconds(waitTimeAtPoint);
        isAgentActive = true;
        MoveToNextPatrolPoint();
    }

    // NOVO: Corotina para quando o inimigo termina de investigar um local
    private IEnumerator FinishInvestigation()
    {
        isAgentActive = false; // Pausa o Update para não chamar a corotina várias vezes
        Debug.Log("<color=lightblue>Inimigo chegou ao ponto de investigação. Esperando...</color>");
        yield return new WaitForSeconds(waitTimeAtPoint); // Reutiliza o tempo de espera da patrulha

        Debug.Log("<color=green>Investigação terminada. Voltando a patrulhar.</color>");
        isAgentActive = true;
        currentState = PatrolState.Patrolling;
        MoveToNextPatrolPoint();
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        int nextPatrolIndex = lastPatrolIndex;
        if (patrolPoints.Length > 1) { while (nextPatrolIndex == lastPatrolIndex) { nextPatrolIndex = Random.Range(0, patrolPoints.Length); } }
        else { nextPatrolIndex = 0; }
        lastPatrolIndex = nextPatrolIndex;
        navMeshAgent.destination = patrolPoints[lastPatrolIndex].position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SongArea"))
        {
            StopAllCoroutines();
            isAgentActive = true;
            currentState = PatrolState.Hunting;
            playerTarget = other.transform.parent;
            navMeshAgent.speed = huntingSpeed;
            navMeshAgent.isStopped = false;
            Debug.Log("<color=red>Jogador detectado! Caçando.</color>");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Só inicia a busca se o inimigo estava de fato caçando o jogador
        if (other.CompareTag("SongArea") && currentState == PatrolState.Hunting)
        {
            StartCoroutine(LoseTargetAndSearch());
        }
    }

    private IEnumerator LoseTargetAndSearch()
    {
        currentState = PatrolState.WaitingAfterHunt;
        playerTarget = null;
        navMeshAgent.isStopped = true;
        Debug.Log($"<color=orange>Alvo perdido. Esperando por {waitTimeAfterHunt} segundos...</color>");
        yield return new WaitForSeconds(waitTimeAfterHunt);
        Debug.Log("<color=green>Voltando a patrulhar.</color>");
        navMeshAgent.isStopped = false;
        currentState = PatrolState.Patrolling;
        navMeshAgent.speed = patrolSpeed;
        MoveToNextPatrolPoint();
    }
}

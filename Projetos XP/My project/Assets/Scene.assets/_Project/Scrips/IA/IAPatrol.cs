using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class IAPatrol : MonoBehaviour
{
    // CORREÇÃO: A enumeração precisa ser pública para ser acessada por outros scripts.
    public enum PatrolState
    {
        Patrolling,
        Hunting,
        WaitingAfterHunt,
        Investigating
    }

    [Header("Referências")]
    [SerializeField] private Transform[] patrolPoints;
    private Transform playerTarget;
    private Vector3 investigationTargetPosition;

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

    [SerializeField] public PatrolState currentState;
    // CORREÇÃO: Propriedade pública para que o UI_Manager possa ler o estado atual de forma segura.
    public PatrolState CurrentState => currentState;

    private NavMeshAgent navMeshAgent;
    private int lastPatrolIndex = -1;
    private bool isAgentActive = true;
    private float lastAttackTime = -99f;

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

        if (currentState == PatrolState.Patrolling)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                StartCoroutine(WaitAtPoint());
            }
        }
        else if (currentState == PatrolState.Hunting)
        {
            if (playerTarget != null)
            {
                navMeshAgent.destination = playerTarget.position;
                if (Vector3.Distance(transform.position, playerTarget.position) <= attackDistance)
                {
                    AttackPlayer();
                }
            }
        }
        else if (currentState == PatrolState.Investigating)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                StartCoroutine(FinishInvestigation());
            }
        }
    }

    public void CallToLocation(Transform targetLocation)
    {
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
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.destination = investigationTargetPosition;
    }

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

    private IEnumerator FinishInvestigation()
    {
        isAgentActive = false;
        Debug.Log("<color=lightblue>Inimigo chegou ao ponto de investigação. Esperando...</color>");
        yield return new WaitForSeconds(waitTimeAtPoint);

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

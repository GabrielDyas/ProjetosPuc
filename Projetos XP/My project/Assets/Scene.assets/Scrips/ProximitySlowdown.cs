using UnityEngine;

public class ProximitySlowdown : MonoBehaviour
{
    [Header("Configura��es do Slowdown")]
    [Tooltip("O alvo a partir do qual a dist�ncia ser� medida (o Player).")]
    [SerializeField] private Transform targetToMeasureFrom;
    [Tooltip("A tag usada para identificar os objetos de inimigo na cena.")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Par�metros do Efeito")]
    [Tooltip("A dist�ncia M�XIMA em que o slowdown come�a a ser sentido.")]
    [SerializeField] private float maxDistance = 20f;
    [Tooltip("A dist�ncia M�NIMA para o slowdown ser m�ximo.")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("O multiplicador de velocidade no ponto mais pr�ximo (ex: 0.3 para 30% da velocidade).")]
    [Range(0f, 1f)] // Garante que o valor no Inspector seja um slider de 0 a 1
    [SerializeField] private float minimumSpeedMultiplier = 0.3f;

    // A propriedade p�blica que o script do Player ir� ler.
    public float SpeedMultiplier { get; private set; } = 1f;

    private Transform closestEnemy;

    void Start()
    {
        if (targetToMeasureFrom == null)
        {
            Debug.LogError("O 'targetToMeasureFrom' (Player) n�o foi atribu�do no Inspector!", this);
        }
    }

    void Update()
    {
        if (targetToMeasureFrom == null) return;

        FindClosestEnemy();
        UpdateSlowdownEffect();
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(targetToMeasureFrom.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
    }

    private void UpdateSlowdownEffect()
    {
        if (closestEnemy == null)
        {
            SpeedMultiplier = 1f; // Sem inimigo, velocidade normal.
            return;
        }

        float currentDistance = Vector3.Distance(targetToMeasureFrom.position, closestEnemy.position);

        // Calcula a intensidade do efeito (0 = longe, 1 = perto)
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        // A L�GICA SIMPLIFICADA EST� AQUI:
        // Interpola linearmente entre a velocidade normal (1.0) e a velocidade m�nima.
        SpeedMultiplier = Mathf.Lerp(1f, minimumSpeedMultiplier, intensity);

        // DEBUG: Para voc� ver o valor em tempo real
        Debug.Log($"<color=cyan>Slowdown Intensity: {intensity:F2}, Final Speed Multiplier: {SpeedMultiplier:F2}</color>");
    }

    // O Gizmo continua �til para visualizar as dist�ncias
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (targetToMeasureFrom == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetToMeasureFrom.position, maxDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetToMeasureFrom.position, minDistance);
    }
#endif
}
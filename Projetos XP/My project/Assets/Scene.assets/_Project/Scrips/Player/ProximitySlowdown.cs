using UnityEngine;

public class ProximityDebuff : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O alvo a partir do qual a distância será medida (o Player).")]
    [SerializeField] private Transform player;
    [Tooltip("A tag usada para identificar os objetos de inimigo na cena.")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Parâmetros de Distância")]
    [Tooltip("A distância MÁXIMA em que o efeito começa a ser sentido.")]
    [SerializeField] private float maxDistance = 20f;
    [Tooltip("A distância MÍNIMA para o efeito ser máximo.")]
    [SerializeField] private float minDistance = 5f;

    [Header("Configurações do Slowdown (Base)")]
    [Tooltip("O multiplicador de velocidade médio no ponto mais próximo (ex: 0.4 para 40% da velocidade).")]
    [Range(0f, 1f)]
    [SerializeField] private float baseSlowdownMultiplier = 0.4f;

    [Header("Configurações do Ruído (Flutuação)")]
    [Tooltip("Define o quão instável a velocidade se torna.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float noiseAmount = 0.2f;

    public float SpeedMultiplier { get; private set; } = 1f;

    private Transform closestEnemy;

    void Update()
    {
        if (player == null) return;

        FindClosestEnemy();
        UpdateDebuffEffect();
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(player.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
    }

    private void UpdateDebuffEffect()
    {
        if (closestEnemy == null)
        {
            SpeedMultiplier = 1f; // Sem inimigo, sem efeito.
            return;
        }

        float currentDistance = Vector3.Distance(player.position, closestEnemy.position);

        // 1. Calcula a intensidade do efeito (0 = longe, 1 = perto)
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        // 2. Calcula o slowdown base 
        float centerMultiplier = Mathf.Lerp(1f, baseSlowdownMultiplier, intensity);

        // 3. Calcula o alcance do ruído com base na intensidade
        float currentNoiseRange = Mathf.Lerp(0f, noiseAmount, intensity);

        // 4. Define os limites mínimo e máximo para a flutuação aleatória
        float randomMin = centerMultiplier - currentNoiseRange;
        float randomMax = centerMultiplier + currentNoiseRange;

        // 5. Gera o valor final aleatório e o limita para segurança
        SpeedMultiplier = Random.Range(randomMin, randomMax);
        SpeedMultiplier = Mathf.Clamp(SpeedMultiplier, 0f, 1.5f);
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, maxDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minDistance);
    }
#endif
}
using UnityEngine;

public class ProximitySlowdown : MonoBehaviour
{
    [Header("Configurações do Slowdown")]
    [Tooltip("O alvo a partir do qual a distância será medida (o Player).")]
    [SerializeField] private Transform targetToMeasureFrom;
    [Tooltip("A tag usada para identificar os objetos de inimigo na cena.")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Parâmetros do Efeito")]
    [Tooltip("A distância MÁXIMA em que o slowdown começa a ser sentido.")]
    [SerializeField] private float maxDistance = 20f;
    [Tooltip("A distância MÍNIMA para o slowdown ser máximo.")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("O multiplicador de velocidade no ponto mais próximo (ex: 0.3 para 30% da velocidade).")]
    [Range(0f, 1f)]
    [SerializeField] private float minimumSpeedMultiplier = 0.3f;

    // A propriedade pública que o script do Player irá ler. Inicia em 1 (sem efeito).
    public float SpeedMultiplier { get; private set; } = 1f;

    private Transform closestEnemy;

    void Update()
    {
        // Ponto de falha 1: Se o Player não foi atribuído, não faz nada.
        if (targetToMeasureFrom == null) return;

        FindClosestEnemy();
        UpdateSlowdownEffect();
    }

    private void FindClosestEnemy()
    {
        // Ponto de falha 2: Procura por objetos com a tag. Se não encontrar, a lógica para.
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
        // Se, após a busca, nenhum inimigo foi encontrado, reseta o multiplicador e para.
        if (closestEnemy == null)
        {
            SpeedMultiplier = 1f;
            return;
        }

        float currentDistance = Vector3.Distance(targetToMeasureFrom.position, closestEnemy.position);

        // Ponto de falha 3: Se a distância for maior que maxDistance, intensity será 0.
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        // A lógica de slowdown. Se intensity for 0, o resultado será 1.
        SpeedMultiplier = Mathf.Lerp(1f, minimumSpeedMultiplier, intensity);
    }

    // Desenha as esferas de distância no editor para fácil visualização.
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
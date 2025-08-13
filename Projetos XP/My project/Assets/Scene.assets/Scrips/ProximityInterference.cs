using UnityEngine;

public class ProximityInterference : MonoBehaviour
{
    [Header("Configurações da Interferência")]
    [Tooltip("A tag usada para identificar os objetos de inimigo na cena.")]
    [SerializeField] private string enemyTag = "Enemy";
    [Tooltip("A distância MÁXIMA em que o efeito começa a ser sentido.")]
    [SerializeField] private float maxDistance = 20f;
    [Tooltip("A distância MÍNIMA para o efeito ser máximo.")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("O valor MÍNIMO do multiplicador de velocidade (ex: 0.5).")]
    [SerializeField] private float minInterference = 0.5f;
    [Tooltip("O valor MÁXIMO do multiplicador de velocidade (ex: 1.5).")]
    [SerializeField] private float maxInterference = 1.5f;

    // Esta propriedade pública permite que outros scripts leiam o valor do multiplicador.
    // O 'private set' significa que apenas este script pode alterar seu valor.
    public float SpeedMultiplier { get; private set; } = 1f;

    private Transform closestEnemy;

    void Update()
    {
        FindClosestEnemy();
        UpdateInterference();
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }
    }

    private void UpdateInterference()
    {
        if (closestEnemy == null)
        {
            SpeedMultiplier = 1f; // Sem inimigos por perto, sem interferência.
            return;
        }

        // Calcula a distância para o inimigo mais próximo
        float currentDistance = Vector3.Distance(transform.position, closestEnemy.position);

        // Calcula a intensidade da interferência (0 a 1)
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        if (intensity <= 0)
        {
            SpeedMultiplier = 1f; // Fora do alcance, sem interferência.
            return;
        }

        // Calcula os limites do ruído aleatório com base na intensidade
        float currentMin = Mathf.Lerp(1f, minInterference, intensity);
        float currentMax = Mathf.Lerp(1f, maxInterference, intensity);

        // Define o valor da propriedade pública com o multiplicador de ruído
        SpeedMultiplier = Random.Range(currentMin, currentMax);
    }
}
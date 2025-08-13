using UnityEngine;

public class ProximityInterference : MonoBehaviour
{
    [Header("Configura��es da Interfer�ncia")]
    [Tooltip("A tag usada para identificar os objetos de inimigo na cena.")]
    [SerializeField] private string enemyTag = "Enemy";
    [Tooltip("A dist�ncia M�XIMA em que o efeito come�a a ser sentido.")]
    [SerializeField] private float maxDistance = 20f;
    [Tooltip("A dist�ncia M�NIMA para o efeito ser m�ximo.")]
    [SerializeField] private float minDistance = 5f;
    [Tooltip("O valor M�NIMO do multiplicador de velocidade (ex: 0.5).")]
    [SerializeField] private float minInterference = 0.5f;
    [Tooltip("O valor M�XIMO do multiplicador de velocidade (ex: 1.5).")]
    [SerializeField] private float maxInterference = 1.5f;

    // Esta propriedade p�blica permite que outros scripts leiam o valor do multiplicador.
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
            SpeedMultiplier = 1f; // Sem inimigos por perto, sem interfer�ncia.
            return;
        }

        // Calcula a dist�ncia para o inimigo mais pr�ximo
        float currentDistance = Vector3.Distance(transform.position, closestEnemy.position);

        // Calcula a intensidade da interfer�ncia (0 a 1)
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        if (intensity <= 0)
        {
            SpeedMultiplier = 1f; // Fora do alcance, sem interfer�ncia.
            return;
        }

        // Calcula os limites do ru�do aleat�rio com base na intensidade
        float currentMin = Mathf.Lerp(1f, minInterference, intensity);
        float currentMax = Mathf.Lerp(1f, maxInterference, intensity);

        // Define o valor da propriedade p�blica com o multiplicador de ru�do
        SpeedMultiplier = Random.Range(currentMin, currentMax);
    }
}
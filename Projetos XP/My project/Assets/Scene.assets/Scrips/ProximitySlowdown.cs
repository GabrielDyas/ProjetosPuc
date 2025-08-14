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
    [Range(0f, 1f)]
    [SerializeField] private float minimumSpeedMultiplier = 0.3f;

    // A propriedade p�blica que o script do Player ir� ler. Inicia em 1 (sem efeito).
    public float SpeedMultiplier { get; private set; } = 1f;

    private Transform closestEnemy;

    void Update()
    {
        // Ponto de falha 1: Se o Player n�o foi atribu�do, n�o faz nada.
        if (targetToMeasureFrom == null) return;

        FindClosestEnemy();
        UpdateSlowdownEffect();
    }

    private void FindClosestEnemy()
    {
        // Ponto de falha 2: Procura por objetos com a tag. Se n�o encontrar, a l�gica para.
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
        // Se, ap�s a busca, nenhum inimigo foi encontrado, reseta o multiplicador e para.
        if (closestEnemy == null)
        {
            SpeedMultiplier = 1f;
            return;
        }

        float currentDistance = Vector3.Distance(targetToMeasureFrom.position, closestEnemy.position);

        // Ponto de falha 3: Se a dist�ncia for maior que maxDistance, intensity ser� 0.
        float intensity = Mathf.InverseLerp(maxDistance, minDistance, currentDistance);

        // A l�gica de slowdown. Se intensity for 0, o resultado ser� 1.
        SpeedMultiplier = Mathf.Lerp(1f, minimumSpeedMultiplier, intensity);
    }

    // Desenha as esferas de dist�ncia no editor para f�cil visualiza��o.
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
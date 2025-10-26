using Unity.VisualScripting;
using UnityEngine;

// O nome da classe é Bomb, então o nome do arquivo deve ser Bomb.cs
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("Raio da explosão.")]
    [SerializeField] private float explosionRadius = 5f;
    [Tooltip("Força da explosão.")]
    [SerializeField] private float explosionForce = 1000f;
    [Tooltip("Dano causado pela explosão.")]
    [Range(1f, 15f)]
    [SerializeField] private float explosionDamege = 13f;

    [Header("Detection Layers")]
    [Tooltip("Layer dos objetos que serão afetados pela explosão.")]
    [SerializeField] private LayerMask explosionLayerMask; 
    [Tooltip("Layer dos obstáculos que podem bloquear a explosão.")]
    [SerializeField] private LayerMask obstacleLayerMask;
    [Tooltip("Indica se a bomba já explodiu.")]
    [SerializeField] private bool _hasExploded = false;
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se a bomba já explodiu ou se colidiu com o chão
        if (_hasExploded || !collision.gameObject.CompareTag("Graund")) return;

        CheckForObjectsInExplosionArea();
        _hasExploded = true;
    }

    // Verifica os objetos na área de explosão
    private void CheckForObjectsInExplosionArea()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);

        Debug.Log($"<color=orange>Explosão ativada! {hitColliders.Length} alvos na área.</color>");

        // Aplica força de explosão e dano aos objetos detectados
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody targetRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // Verifica se há um obstáculo entre a bomba e o objeto
                if (Physics.Raycast(transform.position, directionToTarget.normalized, distanceToTarget, obstacleLayerMask))
                {
                    Debug.Log($"<color=yellow>Objeto {hitCollider.name} está protegido por um obstáculo.</color>");
                    continue; 
                }
                //aplica dano
                _ObjectsData objectData = hitCollider.GetComponent<_ObjectsData>();
                if (objectData != null)
                {
                    // Calcula o dano baseado na distância
                    float damageAmount = Mathf.Clamp(explosionDamege * (1 - (distanceToTarget / explosionRadius)), 0, explosionDamege);
                    objectData.life -= damageAmount;
                    Debug.Log($"<color=red>Objeto {hitCollider.name} recebeu {damageAmount} de dano. Vida restante: {objectData.life}</color>");
                }
                ApplyExplosionForce(targetRigidbody);
            }
        }
    }

    // Aplica a força de explosão ao Rigidbody alvo
    private void ApplyExplosionForce(Rigidbody targetRb)
    {
        targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
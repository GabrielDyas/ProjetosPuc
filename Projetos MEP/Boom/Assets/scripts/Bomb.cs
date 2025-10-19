using Unity.VisualScripting;
using UnityEngine;

// O nome da classe é Bomb, então o nome do arquivo deve ser Bomb.cs
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 1000f;

    [Header("Detection Layers")]
    [SerializeField] private LayerMask explosionLayerMask; // Camada dos alvos (ex: Inimigos, Objetos)
    [SerializeField] private LayerMask obstacleLayerMask;  // <-- NOVO: Camada dos obstáculos (ex: Paredes, Chão)

    [SerializeField] private bool _hasExploded = false;
    private void OnCollisionEnter(Collision collision)
    {
        // Lembrete: A tag "Graund" provavelmente deveria ser "Ground"
        if (_hasExploded || !collision.gameObject.CompareTag("Graund")) return;

        CheckForObjectsInExplosionArea();
        _hasExploded = true;
    }
    private void CheckForObjectsInExplosionArea()
    {
        // 1. Encontra todos os ALVOS na área usando a explosionLayerMask
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);

        Debug.Log($"<color=orange>Explosão ativada! {hitColliders.Length} alvos na área.</color>");

        foreach (var hitCollider in hitColliders)
        {
            Rigidbody targetRigidbody = hitCollider.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // --- LÓGICA DEFINITIVA ---
                // 2. Dispara um raio em direção ao alvo.
                // Este raio SÓ VAI COLIDIR com os OBSTÁCULOS definidos na obstacleLayerMask.
                if (Physics.Raycast(transform.position, directionToTarget.normalized, distanceToTarget, obstacleLayerMask))
                {
                    // Se o raio atingiu qualquer obstáculo, o alvo está protegido.
                    Debug.Log($"<color=yellow>Objeto {hitCollider.name} está protegido por um obstáculo.</color>");
                    continue; // Pula para o próximo alvo
                }

                // 3. Se não houve colisão com obstáculos, há linha de visão direta.
                // Aplica a força da explosão.
                ApplyExplosionForce(targetRigidbody);
            }
        }
    }
    private void ApplyExplosionForce(Rigidbody targetRb)
    {
        targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}


using UnityEngine;
using UnityEngine.InputSystem;

public class ArremeçoBomba : MonoBehaviour
{
    [SerializeField] private GameObject bombaPrefab;
    [SerializeField] private float forçaArremesso = 10f;
    [SerializeField] private float alturaArremesso = 2f;
    [SerializeField] private Direction direction;


    [SerializeField] private Transform pontoArremesso;
    public void Arremessar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ArremessarBomba();
        }
    }
    private void ArremessarBomba()
    {
        if (bombaPrefab == null || pontoArremesso == null)
        {
            Debug.LogWarning("Bomba prefab ou ponto de arremesso não estão atribuídos.");
            return;
        }
        GameObject bombaInstance = Instantiate(bombaPrefab, pontoArremesso.position, Quaternion.identity);
        Rigidbody bombaRigidbody = bombaInstance.GetComponent<Rigidbody>();
        if (bombaRigidbody != null)
        {
            Vector3 direçãoArremesso = direction.directionVector * forçaArremesso + Vector3.up * alturaArremesso;
            bombaRigidbody.AddForce(direçãoArremesso, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("O prefab da bomba não possui um Rigidbody.");
        }
    }
}

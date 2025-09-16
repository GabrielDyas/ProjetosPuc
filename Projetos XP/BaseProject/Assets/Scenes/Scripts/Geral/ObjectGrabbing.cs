using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabbing : MonoBehaviour
{
    [SerializeField] private Transform handPoint;
    [SerializeField] private float grabDistance = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private LayerMask grabbableLayer;
    [Tooltip("Arraste a sua câmera principal (ou a câmera do Cinemachine) para este campo.")]
    [SerializeField] private Camera playerCamera; // Tornamos esta variável pública

    private GameObject grabbedObject = null;
    private Rigidbody grabbedObjectRb = null;

    // O método Awake() não é mais necessário para a câmera, então foi removido.

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Adicionamos uma verificação de segurança para garantir que a câmera foi atribuída
        if (playerCamera == null)
        {
            Debug.LogError("A câmera do jogador não foi atribuída no Inspector do ObjectGrabbing!");
            return;
        }

        if (context.performed)
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
            else
            {
                ThrowObject();
            }
        }
    }

    private void TryGrabObject()
    {
        RaycastHit hit;
        // Ponto de partida do raio, levemente à frente da câmera para evitar colidir com o próprio jogador
        Vector3 rayStartPoint = playerCamera.transform.position + playerCamera.transform.forward * 0.1f;

        // Modificamos a linha abaixo para usar o novo ponto de partida
        if (Physics.Raycast(rayStartPoint, playerCamera.transform.forward, out hit, grabDistance, grabbableLayer))
        {
            if (hit.collider.CompareTag("Ball"))
            {
                grabbedObject = hit.collider.gameObject;
                grabbedObjectRb = grabbedObject.GetComponent<Rigidbody>();

                if (grabbedObjectRb != null)
                {
                    grabbedObjectRb.isKinematic = true;
                    grabbedObject.transform.SetParent(handPoint);
                    grabbedObject.transform.localPosition = Vector3.zero;
                    grabbedObject.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }

    private void ThrowObject()
    {
        if (grabbedObjectRb != null)
        {
            grabbedObject.transform.SetParent(null);
            grabbedObjectRb.isKinematic = false;
            grabbedObjectRb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
        }

        grabbedObject = null;
        grabbedObjectRb = null;
    }
}


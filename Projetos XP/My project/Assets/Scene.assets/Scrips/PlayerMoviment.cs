using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoviment : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] float speedMultiplier;

    [Header("Component References")]
    [Tooltip("Arraste o objeto filho que representa o visual do player.")]
    [SerializeField] private Transform visualChild;
    [Tooltip("A referência ao componente que calcula a interferência de velocidade.")]
    [SerializeField] private ProximityInterference interferenceComponent; // Referência ao novo script

    private CharacterController pcc;
    private Vector2 moveDirection;
    private Vector2 speedDirection;

    void Start()
    {
        pcc = GetComponent<CharacterController>();
        if (interferenceComponent == null)
        {
            Debug.LogWarning("O componente de interferência não foi atribuído.", this);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void ControllSpeed(InputAction.CallbackContext context)
    {
        speedDirection = context.ReadValue<Vector2>();
        ChengeSpeed();
    }

    private void ChengeSpeed()
    {
        if (speedDirection.y != 0)
        {
            if (speed <= 4) { if (speedDirection.y > 0) { speed = speed + 1; } }
            if (speed >= 1) { if (speedDirection.y < 0) { speed = speed - 1; } }
        }
    }

    public void Update()
    {
        if (pcc == null) return;

        // Pega o multiplicador do outro script. Se não houver, usa 1.
        speedMultiplier = (interferenceComponent != null) ? interferenceComponent.SpeedMultiplier : 1f;

        // Lógica de Movimento (agora mais limpa)
        Vector3 move = new Vector3(moveDirection.x, 0f, moveDirection.y);
        pcc.Move(move * (speed * speedMultiplier) * Time.deltaTime);

        // Lógica de Rotação
        if (visualChild != null && moveDirection != Vector2.zero)
        {
            Vector3 rotationDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            visualChild.rotation = Quaternion.Slerp(visualChild.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
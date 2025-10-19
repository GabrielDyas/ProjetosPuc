using UnityEngine;
using UnityEngine.InputSystem;

public class Moviment : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform body;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private CharacterController character;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private Direction direction;
    private float gravity = 9.81f;

    private void LateUpdate()
    {
        IntercectionPoint();
    }
    public void Direction(InputAction.CallbackContext callbackContext)
    { 
        Vector2 inputVector = callbackContext.ReadValue<Vector2>();
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }
    private void Update()
    {
        Vector3 movement = moveDirection * speed * Time.deltaTime;
        movement.y -= gravity * Time.deltaTime;
        character.Move(movement);
        DirecionarOlhar(direction.directionVector);


    }

    private void IntercectionPoint()
    {

    }
    private void DirecionarOlhar(Vector3 pontoAlvo)
    {

        pontoAlvo.y = 0; // Manter a rotação apenas no eixo Y
        if (pontoAlvo.sqrMagnitude > 0.01f) // Evitar rotação desnecessária
        {
            Quaternion targetRotation = Quaternion.LookRotation(pontoAlvo);
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

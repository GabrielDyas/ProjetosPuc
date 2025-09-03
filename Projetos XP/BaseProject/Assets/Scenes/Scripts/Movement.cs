using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [SerializeField] private InputAction control;
    [SerializeField] private float speed;
    [SerializeField] private Transform player ;
    private Vector3 direction;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    public void DirectionMovoment(InputAction.CallbackContext context)
    {
        Vector2 newDirection = context.ReadValue<Vector2>();
        direction = new Vector3(newDirection.y, characterController.s,);
    }

    private void Move()
    {
        characterController.Move(direction*speed*Time.deltaTime);
    }
}

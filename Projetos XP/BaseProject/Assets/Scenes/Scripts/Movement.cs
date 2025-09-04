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
        direction = context.ReadValue<Vector2>();

    }

    private void Move()
    {
        characterController.Move(direction*speed*Time.deltaTime);
    }
}

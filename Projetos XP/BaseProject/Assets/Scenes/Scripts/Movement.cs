using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [SerializeField] private InputAction control;
    [SerializeField] private float velocity;
    private Vector2 direction;

    void Update()
    {    
    }

    public InputAction DirectionMovoment(InputAction.CallbackContext context)
    {
        direction = context;
    }
}

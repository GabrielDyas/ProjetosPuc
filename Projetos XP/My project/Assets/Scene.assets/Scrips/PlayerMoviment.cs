using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField]
    private float speed = 5f; // Speed of the player movement
    [SerializeField]
    private Vector2 speedDirection; // Speed vector to control movement in both x and y directions, can be set by input actions
    [SerializeField]
    private Vector2 moveDirection; // Direction of the player movement, set by input actions
    private CharacterController pcc; // Reference to the CharacterController component
    void Start()
    {
        pcc = GetComponent<CharacterController>(); // Get the CharacterController component attached to the player
        if (pcc == null)
        {
            Debug.LogError("CharacterController component not found on the player object.");
        }
    }

    public void Mover(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>(); // Read the input value from the context

    }
    public void ControllSpeed(InputAction.CallbackContext context)
    {
        speedDirection = context.ReadValue<Vector2>(); // Update the speed variable with the new speed value
        ChengeSpeed();
    }
    private void ChengeSpeed()
    {
        if (speedDirection.y != 0)
        {
            if (speed <= 4)
            {
                if (speedDirection.y > 0)
                {
                    speed = speed + 1; // Set the speed based on the y component of the speedDirection vector
                }
            }
            if (speed >= 1)
            {
                if (speedDirection.y < 0)
                {
                    speed = speed - 1; // Set the speed based on the y component of the speedDirection vector
                }
            }
        }
    }
    public void Update()
    {
        if (pcc != null)
        {
            Vector3 move = new Vector3(moveDirection.x, 0f, moveDirection.y); // Create a movement vector based on input
            pcc.Move(move * speed * Time.deltaTime); // Move the player using the CharacterController


        }
    }
}

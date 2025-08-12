    using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool isGrounded;
    public Transform groundCheck;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            rb.linearVelocity = new Vector2(-5, rb.linearVelocity.y);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            rb.linearVelocity = new Vector2(5, rb.linearVelocity.y);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5);
        }
    }

    private void FixedUpdate()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position))
        {
            isGrounded = true;      
            Debug.DrawLine(transform.position, groundCheck.position, Color.green);
        }
        else
        {
            isGrounded = false;
            Debug.DrawLine(transform.position, groundCheck.position, Color.red);
        }
    }
}

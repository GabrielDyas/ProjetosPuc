using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collision))]
public class Moviment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float deshForce = 15f;
    [SerializeField] private float dashTime = 0.2f;
    private bool isDashing = false;
    private float originalGravityScale;
    private bool canAirDash = true;

    [Header("Ajustes de Colisão (Quinas)")]
    [SerializeField] private Tilemap map;
    [SerializeField] private BoxCollider2D bottomSide; // Sensor na parte de baixo
    [SerializeField] private BoxCollider2D topLeftSide; // Sensor no canto superior esquerdo
    [SerializeField] private BoxCollider2D topRightSide; // Sensor no canto superior direito

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float constantJumpForce = 5f;
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    private bool isJumpPressed = false;


    private Rigidbody2D rb;
    private Collision collision;
    private Vector2 moveInput;
    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>();
        originalGravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {

            return;
        }

        // Lógica normal fora do dash
        if (!wasGrounded && collision.onGround)
        {
            canAirDash = true;
        }
        wasGrounded = collision.onGround;

        TestCoiot();
        Flip();
        Move();
        ApplyConstantJumpForce();
    }


    //Funções de movimentação básica
    private void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }
    private void Flip()
    {
        if (moveInput.x != 0)
        {
            float direction = Mathf.Sign(moveInput.x);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
        }
    }
    public void Direction(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //Funções do dash
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            if (collision.onGround)
            {
                PerformDash();
            }
            else if (canAirDash)
            {
                PerformDash();
                canAirDash = false;
            }
        }
    }

    private void PerformDash()
    {
        isDashing = true;
        rb.gravityScale = 0f;
        Vector2 dashDirection = moveInput.normalized;

        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        }

        rb.linearVelocity = dashDirection * deshForce;
        Invoke("StopDash", dashTime);
    }
    private void StopDash()
    {
        rb.gravityScale = originalGravityScale;
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
    }


    //Funções do jump 
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y); 
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = true;
            coyoteTimeCounter = 0f;
        }

        if (context.canceled)
        {
            isJumpPressed = false;
        }
    }
    private void TestCoiot()
    {
        if (collision.onGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }
    private void ApplyConstantJumpForce()
    {
        if (isJumpPressed && rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector2.up * constantJumpForce, ForceMode2D.Force);
        }
        else
        {
            isJumpPressed = false;
        }
    }
}

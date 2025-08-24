using UnityEngine;
using UnityEngine.InputSystem;

// Garante que o script Collision esteja no mesmo objeto
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

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float constantJumpForce = 5f;
    private bool isJumpPressed = false;

    // Referências para outros componentes
    private Rigidbody2D rb;
    private Collision collision; // Referência para o seu novo script de colisão
    private Vector2 moveInput;
    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<Collision>(); // Pega a referência do script Collision
        originalGravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        // Lógica para resetar o dash aéreo ao tocar o chão
        if (!wasGrounded && collision.onGround)
        {
            canAirDash = true;
        }
        wasGrounded = collision.onGround;

        Flip();
        Move();
        ApplyConstantJumpForce();
    }

    private void Move()
    {
        // A propriedade correta para Rigidbody2D é .velocity
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing)
        {
            // Usa a variável onGround do script Collision
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

        // CORREÇÃO: Usa Mathf.Sign para garantir que a direção tenha sempre magnitude 1
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        }

        // CORREÇÃO: Usa .velocity e aplica a força de forma mais limpa
        rb.linearVelocity = dashDirection * deshForce;
        Invoke("StopDash", dashTime);
    }

    private void StopDash()
    {
        rb.gravityScale = originalGravityScale;
        isDashing = false;
        // CORREÇÃO: Usa .velocity
        rb.linearVelocity = Vector2.zero;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Usa a variável onGround do script Collision
        if (context.started && collision.onGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = true;
        }
        if (context.canceled)
        {
            isJumpPressed = false;
        }
    }

    public void Direction(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void ApplyConstantJumpForce()
    {
        // CORREÇÃO: Usa .velocity
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

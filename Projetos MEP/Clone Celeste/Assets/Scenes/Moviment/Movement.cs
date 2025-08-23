using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpForceContinus = 2f; // Força aplicada enquanto o botão é segurado

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 direction;

    // Variável para rastrear se o botão de pulo está pressionado
    private bool isJumpPressed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = true;
    }

    public void Move()
    {
        // A normalização não é necessária aqui, o Input System já lida com isso para gamepads.
        // Se estiver usando apenas teclado, não faz diferença.
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
    }

    public void Direction(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Pulo inicial, ocorre apenas uma vez quando o botão é pressionado no chão
        if (context.started && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            isJumpPressed = true; // O botão foi pressionado
        }

        // Quando o botão é solto, paramos de aplicar a força extra
        if (context.canceled)
        {
            isJumpPressed = false; // O botão foi solto
        }
    }

    private void FixedUpdate()
    {
        Move();

        // Aplica a força extra enquanto o botão estiver pressionado e o personagem estiver subindo
        if (isJumpPressed && rb.linearVelocity.y > 0)
        {
            // Usamos ForceMode2D.Force para aplicar uma força contínua
            rb.AddForce(Vector2.up * jumpForceContinus, ForceMode2D.Force);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Uma verificação mais robusta para o chão é recomendada, mas esta funciona para casos simples
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

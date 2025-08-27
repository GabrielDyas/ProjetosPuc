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
            // A lógica de correção de quina do dash é chamada aqui
            CornerCorrect();
            return;
        }

        // Lógica normal fora do dash
        wasGrounded = collision.onGround;
        if (!wasGrounded && collision.onGround)
        {
            canAirDash = true;
        }

        TestCoiot();
        Flip();
        Move();
        ApplyConstantJumpForce();
        CornerCorrect(); // Chama a correção para as quinas superiores
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

    // NOVA FUNÇÃO PARA CORREÇÃO DE QUINAS
    private void CornerCorrect()
    {
        // --- Correção de Quina Inferior (SOMENTE DURANTE O DASH) ---
        if (isDashing)
        {
            Vector2 ajust = collision.GetTilemapIntersectionSize(map, bottomSide);
            // Se a colisão tem largura, mas é menor que a largura total do sensor (é uma quina)
            if (ajust.x > 0 && ajust.x < bottomSide.size.x)
            {
                // Ajusta a posição para cima para "desprender" da quina
                transform.position += new Vector3(0, ajust.y, 0);
            }



        }

        // --- Correção de Quina Superior (A QUALQUER MOMENTO) ---
        // Verifica o canto superior direito
        Vector2 ajustRight = collision.GetTilemapIntersectionSize(map, topRightSide);
        // Se a colisão tem altura, mas é menor que a altura total do sensor (é uma quina)
        if (ajustRight.y > 0 && ajustRight.y < topRightSide.size.y)
        {
            // Ajusta a posição para cima para subir na plataforma
            transform.position += new Vector3(0, topRightSide.size.y - ajustRight.y, 0);
        }

        // Verifica o canto superior esquerdo
        Vector2 ajustLeft = collision.GetTilemapIntersectionSize(map, topLeftSide);
        if (ajustLeft.y > 0 && ajustLeft.y < topLeftSide.size.y)
        {
            transform.position += new Vector3(0, topLeftSide.size.y - ajustLeft.y, 0);
        }
    }

    //Funções do jump 
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Zera a velocidade Y para um pulo consistente
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpPressed = true;
            coyoteTimeCounter = 0f; // Zera o contador para evitar pulos duplos
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

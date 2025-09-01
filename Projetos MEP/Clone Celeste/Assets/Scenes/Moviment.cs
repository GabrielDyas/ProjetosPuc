using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collision))]
public class Moviment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float stamina = 5f;
    private float directionMove;
    private float consumStamina;
    private bool isClimbing = false;
    private bool isClimbInputHeld = false; 

    [Header("Dash Settings")]
    [SerializeField] private float deshForce = 15f;
    [SerializeField] private float dashTime = 0.2f;
    private bool isDashing = false;
    private float originalGravityScale;
    private bool canAirDash = true;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallJumpForce = 12f;
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
        consumStamina = stamina;
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

        HandleClimbing();

        HandleDashReset();

        TestCoiot();
        Flip();
        Move();
        ApplyConstantJumpForce();
    }
    // --- Funções de Movimentação ---
    private void Move()
    {
        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, moveInput.y * climbSpeed);
            if (directionMove.y != 0)
            {

            }
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        if (moveInput.x != 0 && !isClimbing)
        {
            directionMove = Mathf.Sign(moveInput.x);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * directionMove, transform.localScale.y, transform.localScale.z);
        }
    }

    public void Direction(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // --- Funções de Escalada ---
    private void HandleClimbing()
    {
        if (isClimbInputHeld && collision.onWall)
        {
            isClimbing = true;
            rb.gravityScale = 0f;
        }
        else
        {
            if (isClimbing)
            {
                rb.gravityScale = originalGravityScale;
            }
            isClimbing = false;
        }
    }

    public void Climbing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isClimbInputHeld = true;
        }
        else if (context.canceled)
        {
            isClimbInputHeld = false;
        }
    }

    // --- Funções do Dash ---
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !isDashing && !isClimbing)
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

    private void HandleDashReset()
    {
        if (!wasGrounded && collision.onGround)
        {
            canAirDash = true;
        }
        wasGrounded = collision.onGround;
    }

    // --- Funções do Pulo ---
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if ((isClimbing || collision.onWall) && !collision.onGround)
            {
                WallJump();
            }
            else if (coyoteTimeCounter > 0f)
            {
                GroundJump();
            }
        }

        if (context.canceled)
        {
            isJumpPressed = false;
        }
    }

    private void GroundJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumpPressed = true;
        coyoteTimeCounter = 0f;
    }

    private void WallJump()
    {
        isClimbing = false;
        isClimbInputHeld = false;
        rb.gravityScale = originalGravityScale;

        float wallDirection = collision.onRightWall ? -1 : 1;
        Vector2 jumpDirection = new Vector2(wallDirection, 1).normalized;

        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
        isJumpPressed = true;
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


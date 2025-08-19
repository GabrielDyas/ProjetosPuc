using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoviment : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] float speedMultiplier;
    [SerializeField] public float finalSpeed;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    public int VidaAtual => currentHealth;

    [Header("Song Area Settings")]
    [SerializeField] private float songAreaBaseSize = 1.5f;
    [SerializeField] private float songAreaSpeedContribution = 1.0f;

    [Header("Component References")]
    [SerializeField] private Transform visualChild;
    [SerializeField] private ProximityDebuff si;
    [SerializeField] private Transform songArea;
    [SerializeField] private AltarManager altarManager;
    [SerializeField] private UI_Manager uiManager;

    private CharacterController pcc;
    private Vector2 moveDirection;
    private Vector2 speedDirection;
    private Animator animator; // NOVO: Referência para o Animator

    void Start()
    {
        pcc = GetComponent<CharacterController>();

        // NOVO: Pega o componente Animator que está no objeto visual
        if (visualChild != null)
        {
            animator = visualChild.GetComponent<Animator>();
        }

        if (si == null)
        {
            Debug.LogWarning("O componente de interferência não foi atribuído.", this);
        }

        currentHealth = maxHealth;
    }

    public void Update()
    {
        Movimente();
        UpdateSongArea();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"<color=orange>Player tomou {damageAmount} de dano! Vidas restantes: {currentHealth}</color>");

        if (currentHealth <= 0)
        {
            Debug.Log("<color=red>Player morreu.</color>");
            this.enabled = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("PortaDeSaida"))
        {
            if (altarManager != null && altarManager.PortasAbertas)
            {
                if (uiManager != null)
                {
                    uiManager.MostrarTelaFinal();
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void ControllSpeed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speedDirection = context.ReadValue<Vector2>();
            ChengeSpeed();
        }
    }

    private void ChengeSpeed()
    {
        if (speedDirection.y != 0)
        {
            if (speed <= 4) { if (speedDirection.y > 0) { speed = speed + 1; } }
            if (speed >= 1) { if (speedDirection.y < 0) { speed = speed - 1; } }
        }
    }

    private void Movimente()
    {
        if (pcc == null) return;
        float speedMultiplier = (si != null) ? si.SpeedMultiplier : 1f;
        Vector3 move = new Vector3(moveDirection.x, 0f, moveDirection.y);
        Vector3 finalVelocity = move.normalized * speed * speedMultiplier;
        finalSpeed = finalVelocity.magnitude;
        pcc.Move(finalVelocity * Time.deltaTime);

        // NOVO: Atualiza o parâmetro "Speed" no Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", finalSpeed);
        }

        if (visualChild != null && moveDirection != Vector2.zero)
        {
            Vector3 rotationDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            visualChild.rotation = Quaternion.Slerp(visualChild.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateSongArea()
    {
        if (songArea != null)
        {
            float newScale = songAreaBaseSize + (finalSpeed * songAreaSpeedContribution);
            newScale = Mathf.Max(0, newScale);
            songArea.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}

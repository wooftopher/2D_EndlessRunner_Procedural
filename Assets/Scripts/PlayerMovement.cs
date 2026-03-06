using UnityEngine;
using UnityEngine.InputSystem; // new Input System

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;       // How fast the player moves up per frame
    [SerializeField] private float maxJumpTime = 0.3f;    // Maximum time jump can be held
    [SerializeField] private LayerMask groundLayer;       // What counts as ground
    [SerializeField] private Transform groundCheck;       // Empty child at feet
    [SerializeField] private float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;

    private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    // Input actions asset
    private PlayerInputActions controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInputActions();
    }

    void OnEnable()
    {
        controls.Player.Enable();

        // Left/right movement
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Jump input
        controls.Player.Jump.performed += ctx =>
        {
            if (isGrounded)
            {
                isJumping = true;
                jumpTimeCounter = maxJumpTime;
            }
        };
        controls.Player.Jump.canceled += ctx => isJumping = false;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void FixedUpdate()
    {
        // Left/right movement
        rb.MovePosition(rb.position + moveInput.x * moveSpeed * Vector2.right * Time.fixedDeltaTime);

        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump movement (variable height)
        if (isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.position += Vector2.up * jumpForce * Time.fixedDeltaTime;
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Debug logs
        Debug.Log("Grounded: " + isGrounded + " | Jumping: " + isJumping);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
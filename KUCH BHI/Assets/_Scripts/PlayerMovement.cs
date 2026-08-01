using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    private PlayerPushPull pushPull;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pushPull = GetComponent<PlayerPushPull>(); // Grab reference
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Only jump if not currently grabbing a heavy object
        if (Input.GetButtonDown("Jump") && isGrounded && (pushPull == null || !pushPull.IsGrabbing))
        {
            jumpRequested = true;
        }

        // DO NOT flip character sprite if grabbing/pulling an object
        if (pushPull == null || !pushPull.IsGrabbing)
        {
            if (horizontalInput > 0)
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
        }
    }

    private void FixedUpdate()
    {
        // 1. Check if standing on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Apply horizontal velocity
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // 3. Apply jump force if requested
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    // Visualize the ground check circle in the Scene view for easy debugging
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
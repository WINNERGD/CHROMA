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

    // The base size of your player (0.1f)
    public float BaseScaleMagnitude { get; private set; } = 0.1f;

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    private PlayerPushPull pushPull;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pushPull = GetComponent<PlayerPushPull>();
        anim = GetComponent<Animator>();

        // Automatically store initial Y scale magnitude (0.1f)
        BaseScaleMagnitude = Mathf.Abs(transform.localScale.y);
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // --- ANIMATION CONTROL ---
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;
            anim.speed = isMoving ? 1f : 0f;
        }

        // Only jump if grounded and not grabbing an object
        if (Input.GetButtonDown("Jump") && isGrounded && (pushPull == null || !pushPull.IsGrabbing))
        {
            jumpRequested = true;
        }

        // --- SPRITE FLIPPING ---
        // Adjust sprite direction while preserving current scale proportions (even on platforms)
        if (pushPull == null || !pushPull.IsGrabbing)
        {
            if (horizontalInput > 0)
            {
                SetFacingDirection(1f);
            }
            else if (horizontalInput < 0)
            {
                SetFacingDirection(-1f);
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    // Flips the horizontal scale cleanly while accounting for platform parenting
    public void SetFacingDirection(float direction)
    {
        Vector3 currentScale = transform.localScale;
        float sign = Mathf.Sign(direction);

        transform.localScale = new Vector3(
            Mathf.Abs(currentScale.x) * sign,
            currentScale.y,
            currentScale.z
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
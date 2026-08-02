using UnityEngine;

public class PlateActivatedPlatform : MonoBehaviour
{
    [Header("Movement Range")]
    [SerializeField] private float moveDistance = 4f; // Distance to move horizontally (Left to Right)
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool moveOnlyWhenActive = true; // True: pauses when box removed; False: stays active permanently once triggered

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isActive = false;
    private float progress = 0f;

    private void Start()
    {
        startPos = transform.position;
        // Target position directly to the right of start position
        targetPos = startPos + Vector3.right * moveDistance;
    }

    private void Update()
    {
        if (isActive)
        {
            // Smoothly ping-pong between startPos (left) and targetPos (right)
            progress += Time.deltaTime * speed;
            float t = Mathf.PingPong(progress, 1f);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
        }
        else if (moveOnlyWhenActive)
        {
            // Smoothly return to original resting position when box is taken off
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
        }
    }

    // --- ACTIVATION METHODS (Called by PressurePlate UnityEvents) ---
    public void ActivatePlatform()
    {
        isActive = true;
    }

    public void DeactivatePlatform()
    {
        if (moveOnlyWhenActive)
        {
            isActive = false;
        }
    }

    // --- NO-DISTORTION PLAYER PARENTING ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            float baseScale = (player != null) ? player.BaseScaleMagnitude : 0.1f;

            // Preserve current player facing direction (+1 or -1)
            float facingDir = Mathf.Sign(collision.transform.localScale.x);

            // Parent player to platform
            collision.transform.SetParent(transform, true);

            // Fetch platform scale (e.g., x = 2.0, y = 0.5, z = 0)
            Vector3 parentScale = transform.localScale;

            // Prevent division by zero if Z scale is 0
            float safeZ = parentScale.z != 0 ? parentScale.z : 1f;

            // Adjust local scale so world space size remains crisp 0.1 x 0.1 x 0.1
            collision.transform.localScale = new Vector3(
                (baseScale / parentScale.x) * facingDir,
                baseScale / parentScale.y,
                baseScale / safeZ
            );
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            float baseScale = (player != null) ? player.BaseScaleMagnitude : 0.1f;

            // Unparent from platform
            collision.transform.SetParent(null);

            // Restore exact un-parented 0.1 base scale while preserving facing direction
            float facingDir = Mathf.Sign(collision.transform.localScale.x);
            collision.transform.localScale = new Vector3(baseScale * facingDir, baseScale, baseScale);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 from = Application.isPlaying ? startPos : transform.position;
        Vector3 to = from + Vector3.right * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireCube(to, Vector3.one * 0.3f);
    }
}
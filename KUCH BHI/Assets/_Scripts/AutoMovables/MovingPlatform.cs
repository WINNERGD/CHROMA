using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Range")]
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveDistance;
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(startPos, targetPos, t);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            float baseScale = (player != null) ? player.BaseScaleMagnitude : 0.1f;

            // Preserve current facing direction (+1 or -1)
            float facingDir = Mathf.Sign(collision.transform.localScale.x);

            // Parent player to platform
            collision.transform.SetParent(transform, true);

            // Fetch platform parent scale (x = 2.0, y = 0.5, z = 0)
            Vector3 parentScale = transform.localScale;

            // Prevent division by zero for Z-axis scale
            float safeZ = parentScale.z != 0 ? parentScale.z : 1f;

            // Compensate local scale so world representation stays (0.1 x 0.1 x 0.1)
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
        Vector3 to = from + Vector3.up * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawWireCube(to, Vector3.one * 0.3f);
    }
}
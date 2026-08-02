using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AggressivePatrolEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Agro / Chase Settings")]
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float chaseSpeed = 4.5f;

    private Transform currentTarget;
    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        currentTarget = pointB;
    }

    private void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= detectRange)
        {
            // CHASE STATE
            ChasePlayer();
        }
        else
        {
            // PATROL STATE
            Patrol();
        }
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null) return;

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        // Switch direction when reaching waypoint
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.3f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}

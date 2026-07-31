using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PassivePatrolEnemy : MonoBehaviour
{
    [Header("Patrol Boundaries")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2.5f;

    private Transform currentTarget;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB;
    }

    private void Update()
    {
        if (pointA == null || pointB == null) 
            return;

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite to face patrol direction
        if (direction.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }

        // Swap target when reaching endpoint
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.3f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
}

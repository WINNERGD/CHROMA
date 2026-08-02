using System.Collections;
using UnityEngine;

public class SuicideEnemy : MonoBehaviour
{
    [Header("Detection & Speeds")]
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private float explodeRange = 1.2f;
    [SerializeField] private float moveSpeed = 5f;

    [Header("Spore Cloud Prefab")]
    [SerializeField] private GameObject sporeCloudPrefab;

    private Transform player;
    private Rigidbody2D rb;
    private bool isExploding = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null || isExploding) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= explodeRange)
        {
            Explode();
        }
        else if (distToPlayer <= detectRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void Explode()
    {
        isExploding = true;
        rb.linearVelocity = Vector2.zero;

        // Instant explosion damage (1 hit = 25%)
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, explodeRange, LayerMask.GetMask("Player"));
        if (playerCollider != null && playerCollider.TryGetComponent<PlayerHealth>(out PlayerHealth health))
        {
            health.TakeDirectDamage(1);
        }

        if (sporeCloudPrefab != null)
        {
            Instantiate(sporeCloudPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}
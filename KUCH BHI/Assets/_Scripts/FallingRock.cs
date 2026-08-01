using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingRock : MonoBehaviour
{
    [Header("Rock Settings")]
    [SerializeField] private int damageHits = 1; // 1 Hit = 25% grey damage
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float lifetimeAfterImpact = 0.2f;

    [Header("Effects")]
    [SerializeField] private GameObject impactEffectPrefab;

    private bool hasHit = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasHit = true;

            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamageWithKnockback(damageHits, transform.position, knockbackForce);
            }

            DestroyRock();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHit = true;
            DestroyRock();
        }
    }

    private void DestroyRock()
    {
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, lifetimeAfterImpact);
    }
}
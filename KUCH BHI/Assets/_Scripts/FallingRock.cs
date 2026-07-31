using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingRock : MonoBehaviour
{
    [Header("Rock Settings")]
    [SerializeField] private float damageAmount = 1f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float lifetimeAfterImpact = 0.2f;

    [Header("Effects")]
    [SerializeField] private GameObject impactEffectPrefab; // Optional visual hit effect

    private bool hasHit = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            hasHit = true;

            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                // Deal 1 damage + apply knockback and trigger 1s invulnerability
                health.TakeDamageWithKnockback(damageAmount, transform.position, knockbackForce);
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

        // Destroy rock upon impact after a split second
        Destroy(gameObject, lifetimeAfterImpact);
    }
}

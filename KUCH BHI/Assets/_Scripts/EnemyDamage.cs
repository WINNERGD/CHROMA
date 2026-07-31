using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float knockbackForce = 8f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                // Passes damage, attacker position, and knockback force
                health.TakeDamageWithKnockback(damageAmount, transform.position, knockbackForce);
            }
        }
    }
}

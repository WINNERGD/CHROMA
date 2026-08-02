using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damageHits = 1; // 1 hit = 25% grey damage
    [SerializeField] private float knockbackForce = 8f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamageWithKnockback(damageHits, transform.position, knockbackForce);
            }
        }
    }
}
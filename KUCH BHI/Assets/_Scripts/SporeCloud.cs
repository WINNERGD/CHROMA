using UnityEngine;

public class SporeCloud : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 1f;
    [SerializeField] private float cloudDuration = 5f;

    private void Start()
    {
        Destroy(gameObject, cloudDuration);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                // Direct continuous damage: bypasses invulnerability and knockback
                health.TakeDirectDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}

using UnityEngine;

public class SporeCloud : MonoBehaviour
{
    [SerializeField] private float damageInterval = 1.0f; // Deals 1 hit every second in cloud
    [SerializeField] private float cloudDuration = 5f;

    private float timer = 0f;

    private void Start()
    {
        Destroy(gameObject, cloudDuration);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer += Time.deltaTime;
            if (timer >= damageInterval)
            {
                if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
                {
                    health.TakeDirectDamage(1); // 1 Hit (25%) per interval
                }
                timer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
        }
    }
}
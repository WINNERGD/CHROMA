using UnityEngine;

public class RedCrystalItem : MonoBehaviour
{
    [Header("Floating Animation (Optional)")]
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private float bounceHeight = 0.2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Simple gentle floating animation
        float newY = startPos.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScarf scarf = other.GetComponent<PlayerScarf>();
            if (scarf != null)
            {
                scarf.EnableGlowingScarf();
                Destroy(gameObject); // Collect item
            }
        }
    }
}
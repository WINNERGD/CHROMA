using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableBox : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Keep the box stable by default so it doesn't slide away on its own
        rb.mass = 5f;
        rb.linearDamping = 5f; // In Unity 6; if using Unity 2022/2021 use rb.drag = 5f;

        // Fix: Rigidbody2DConstraints (not Rigidbody2DConstraints2D)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void ConnectToPlayer(Transform playerTransform)
    {
        transform.SetParent(playerTransform);
    }

    public void DisconnectFromPlayer()
    {
        transform.SetParent(null);
    }
}
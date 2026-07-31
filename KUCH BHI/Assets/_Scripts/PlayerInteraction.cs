using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform interactCheckPoint;
    [SerializeField] private float interactRadius = 0.8f;
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        // Find any colliders in range on the interactable layer
        Collider2D hit = Physics2D.OverlapCircle(interactCheckPoint.position, interactRadius, interactableLayer);

        if (hit != null)
        {
            // Check if the object has an interactable script
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactCheckPoint.position, interactRadius);
        }
    }
}
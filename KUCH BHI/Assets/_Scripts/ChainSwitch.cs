using UnityEngine;

public class ChainSwitch : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    [SerializeField] private GameObject chainVisual; // The actual chain link to hide/destroy
    [SerializeField] private SpriteRenderer switchSprite;
    [SerializeField] private Color activatedColor = Color.green;

    private bool isReleased = false;

    public void Interact()
    {
        if (isReleased) return;

        isReleased = true;

        // Hide or destroy the chain graphic
        if (chainVisual != null) chainVisual.SetActive(false);
        if (switchSprite != null) switchSprite.color = activatedColor;

        // Notify the Boss Manager that a chain broke
        if (BossManager.Instance != null)
        {
            BossManager.Instance.BreakChain();
        }
    }
}

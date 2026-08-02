using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField] private BossController bossController;
    [SerializeField] private SpriteRenderer buttonSpriteRenderer;
    [SerializeField] private Color activatedColor = Color.green;

    private bool isAlreadyPressed = false;

    public void Interact()
    {
        if (isAlreadyPressed) return;

        isAlreadyPressed = true;

        if (buttonSpriteRenderer != null)
        {
            buttonSpriteRenderer.color = activatedColor;
        }

        if (bossController != null)
        {
            bossController.OnChainButtonPressed();
        }
    }
}
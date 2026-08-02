using UnityEngine;

public class PlayerScarf : MonoBehaviour
{
    [Header("Scarf Visual References")]
    [SerializeField] private GameObject normalScarfVisual;
    [SerializeField] private GameObject glowingScarfVisual;

    [Header("State")]
    public bool hasRedCrystal { get; private set; } = false;

    private void Start()
    {
        // Default state: Normal scarf active, glowing scarf hidden
        if (glowingScarfVisual != null) glowingScarfVisual.SetActive(false);
        if (normalScarfVisual != null) normalScarfVisual.SetActive(true);
    }

    public void EnableGlowingScarf()
    {
        hasRedCrystal = true;

        if (normalScarfVisual != null) normalScarfVisual.SetActive(false);
        if (glowingScarfVisual != null) glowingScarfVisual.SetActive(true);

        Debug.Log("<color=red>[PlayerScarf]</color> Red Crystal collected! Scarf is now glowing.");
    }
}
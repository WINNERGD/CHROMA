using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("4-Hit Health System")]
    [SerializeField] private int maxHealth = 4;
    private int currentHealth;

    [Header("UI & Visual References")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Image uiGreyFillImage; // UI Image set to Image Type: Filled (Vertical, Bottom)
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullGreyColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnerabilityDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;
    private bool isInvulnerable = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        UpdateHealthVisuals();
    }

    /// <summary>
    /// Call when player takes a hit with knockback (e.g. rocks, patrolling enemies)
    /// </summary>
    public void TakeDamageWithKnockback(float damageAmount, Vector2 attackerPosition, float knockbackForce)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        int hits = Mathf.Max(1, Mathf.RoundToInt(damageAmount));
        currentHealth = Mathf.Clamp(currentHealth - hits, 0, maxHealth);

        UpdateHealthVisuals();

        // Apply knockback force
        float knockbackDir = Mathf.Sign(transform.position.x - attackerPosition.x);
        if (knockbackDir == 0) knockbackDir = 1f;
        Vector2 force = new Vector2(knockbackDir * knockbackForce, knockbackForce * 0.5f);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    /// <summary>
    /// Call for direct/continuous damage (e.g. spores, boss laser)
    /// </summary>
    public void TakeDirectDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;

        int hits = Mathf.Max(1, Mathf.RoundToInt(damageAmount));
        currentHealth = Mathf.Clamp(currentHealth - hits, 0, maxHealth);

        UpdateHealthVisuals();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthVisuals()
    {
        // Calculate health percentage:
        // 4 Hits Left = 1.0 (0% Grey)
        // 3 Hits Left = 0.75 (25% Grey)
        // 2 Hits Left = 0.50 (50% Grey)
        // 1 Hit Left  = 0.25 (75% Grey)
        // 0 Hits Left = 0.00 (100% Fully Grey)
        float healthPercent = (float)currentHealth / maxHealth;
        float greyPercent = 1f - healthPercent;

        // 1. Update UI Indicator (Fills grey from bottom to top as health decreases)
        if (uiGreyFillImage != null)
        {
            uiGreyFillImage.fillAmount = greyPercent;
        }

        // 2. Tint Player Sprite toward Grey based on damage taken
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = Color.Lerp(normalColor, fullGreyColor, greyPercent);
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsed = 0f;

        while (elapsed < invulnerabilityDuration)
        {
            if (playerSpriteRenderer != null)
            {
                playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;
            }
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player Dead - 4 Hits Taken");
        UpdateHealthVisuals();

        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
    }
}

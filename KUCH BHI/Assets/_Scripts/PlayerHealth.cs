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
    [SerializeField] private Image uiGreyFillImage; // UI Image Type: Filled (Vertical, Bottom)

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullGreyColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnerabilityDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.1f;
    private bool isInvulnerable = false;

    [Header("Respawn Settings")]
    [SerializeField] private Transform spawnPoint; // Drag your SpawnPoint GameObject here
    [SerializeField] private float respawnDelay = 0.5f; // Short pause before respawning
    private Vector3 initialSpawnPosition;
    private bool isDead = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Store initial position in case spawnPoint transform isn't assigned
        if (spawnPoint != null)
        {
            initialSpawnPosition = spawnPoint.position;
        }
        else
        {
            initialSpawnPosition = transform.position;
        }

        UpdateHealthVisuals();
    }

    public void TakeDamageWithKnockback(int damageHits, Vector2 attackerPosition, float knockbackForce)
    {
        if (isInvulnerable || isDead || currentHealth <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damageHits, 0, maxHealth);
        UpdateHealthVisuals();

        // Knockback Impulse
        float knockbackDir = Mathf.Sign(transform.position.x - attackerPosition.x);
        if (knockbackDir == 0) knockbackDir = 1f;
        Vector2 force = new Vector2(knockbackDir * knockbackForce, knockbackForce * 0.5f);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            DieAndRespawn();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    public void TakeDirectDamage(int damageHits)
    {
        if (isDead || currentHealth <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damageHits, 0, maxHealth);
        UpdateHealthVisuals();

        if (currentHealth <= 0)
        {
            DieAndRespawn();
        }
    }

    private void UpdateHealthVisuals()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        float greyPercent = 1f - healthPercent;

        // 1. UI Health Bar fill
        if (uiGreyFillImage != null)
        {
            uiGreyFillImage.fillAmount = greyPercent;
        }

        // 2. Player Sprite Tint Lerp
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

    private void DieAndRespawn()
    {
        if (isDead) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isDead = true;

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Wait brief delay for feedback
        yield return new WaitForSeconds(respawnDelay);

        // Move to Spawn Point
        Vector3 targetRespawnPosition = spawnPoint != null ? spawnPoint.position : initialSpawnPosition;
        transform.position = targetRespawnPosition;

        // Reset Physics
        rb.linearVelocity = Vector2.zero;

        // Reset Health & Visuals
        currentHealth = maxHealth;
        isDead = false;
        UpdateHealthVisuals();

        // Grant temporary invulnerability post-respawn so player isn't instantly hit again
        StartCoroutine(InvulnerabilityRoutine());
    }

    // Optional: Call this from a Checkpoint script to update spawn position dynamically
    public void SetNewSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }
}
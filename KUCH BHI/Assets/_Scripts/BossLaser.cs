using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [Header("Laser Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float damagePerSecond = 35f;

    [Header("Telegraph & Attack Timings")]
    [Tooltip("How long the thin warning line shows before the full laser fires")]
    [SerializeField] private float telegraphDuration = 1.2f;
    [Tooltip("How long the active lethal beam stays on screen")]
    [SerializeField] private float laserActiveDuration = 1.0f;

    [Header("Colors")]
    [SerializeField] private Color warningColor = new Color(1f, 0.2f, 0.2f, 0.35f); // Semi-transparent Red/Grey
    [SerializeField] private Color activeColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);     // Solid Dark Grey

    private Transform playerTransform;
    private Coroutine attackRoutine;
    private float currentMinInterval = 5f;
    private float currentMaxInterval = 8f;

    private void Awake()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    /// <summary>
    /// Starts the laser routine with randomized delay ranges between attacks.
    /// </summary>
    public void StartRandomLaserCycle(float minInterval, float maxInterval)
    {
        currentMinInterval = minInterval;
        currentMaxInterval = maxInterval;

        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(RandomLaserRoutine());
    }

    public void StopLaser()
    {
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    private IEnumerator RandomLaserRoutine()
    {
        while (true)
        {
            // 1. Pick a random interval delay before starting telegraph
            float randomWait = Random.Range(currentMinInterval, currentMaxInterval);
            yield return new WaitForSeconds(randomWait);

            if (playerTransform == null) yield break;

            // ----------------------------------------------------
            // STEP 2: TELEGRAPH WARNING PHASE
            // ----------------------------------------------------
            lineRenderer.enabled = true;
            lineRenderer.startColor = warningColor;
            lineRenderer.endColor = warningColor;
            lineRenderer.startWidth = 0.05f; // Thin line
            lineRenderer.endWidth = 0.05f;

            float telegraphTimer = telegraphDuration;

            // Target tracks the player's movement during telegraph
            while (telegraphTimer > 0)
            {
                Vector2 aimDirection = (playerTransform.position - firePoint.position).normalized;
                SetLaserPositions(aimDirection);

                telegraphTimer -= Time.deltaTime;
                yield return null;
            }

            // ----------------------------------------------------
            // STEP 3: LETHAL BEAM FIRING PHASE
            // ----------------------------------------------------
            lineRenderer.startColor = activeColor;
            lineRenderer.endColor = activeColor;
            lineRenderer.startWidth = 0.5f; // Thick beam
            lineRenderer.endWidth = 0.5f;

            // Freeze direction at moment of firing
            Vector2 fireDirection = (playerTransform.position - firePoint.position).normalized;

            float activeTimer = laserActiveDuration;
            while (activeTimer > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, fireDirection, maxDistance, hitLayers);
                SetLaserPositions(fireDirection);

                // Apply continuous damage if player touches beam
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    if (hit.collider.TryGetComponent<PlayerHealth>(out PlayerHealth health))
                    {
                        health.TakeDirectDamage(damagePerSecond * Time.deltaTime);
                    }
                }

                activeTimer -= Time.deltaTime;
                yield return null;
            }

            // Disable line after firing until next random cycle
            lineRenderer.enabled = false;
        }
    }

    private void SetLaserPositions(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, maxDistance, hitLayers);
        Vector3 endPoint = hit.collider != null
            ? (Vector3)hit.point
            : firePoint.position + (Vector3)(direction * maxDistance);

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}

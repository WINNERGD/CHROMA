using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [Header("Laser Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float tickInterval = 0.5f; // Deals 1 hit every 0.5s inside beam

    [Header("Telegraph & Attack Timings")]
    [SerializeField] private float telegraphDuration = 0.5f;
    [SerializeField] private float laserActiveDuration = 0.5f;

    [Header("Colors")]
    [SerializeField] private Color warningColor = new Color(1f, 0.2f, 0.2f, 0.35f);
    [SerializeField] private Color activeColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);

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
            float randomWait = Random.Range(currentMinInterval, currentMaxInterval);
            yield return new WaitForSeconds(randomWait);

            if (playerTransform == null) yield break;

            // Telegraph
            lineRenderer.enabled = true;
            lineRenderer.startColor = warningColor;
            lineRenderer.endColor = warningColor;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            float telegraphTimer = telegraphDuration;

            while (telegraphTimer > 0)
            {
                Vector2 aimDirection = (playerTransform.position - firePoint.position).normalized;
                SetLaserPositions(aimDirection);

                telegraphTimer -= Time.deltaTime;
                yield return null;
            }

            // Firing
            lineRenderer.startColor = activeColor;
            lineRenderer.endColor = activeColor;
            lineRenderer.startWidth = 0.5f;
            lineRenderer.endWidth = 0.5f;

            Vector2 fireDirection = (playerTransform.position - firePoint.position).normalized;

            float activeTimer = laserActiveDuration;
            float tickTimer = 0f;

            while (activeTimer > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, fireDirection, maxDistance, hitLayers);
                SetLaserPositions(fireDirection);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    tickTimer += Time.deltaTime;
                    if (tickTimer >= tickInterval)
                    {
                        if (hit.collider.TryGetComponent<PlayerHealth>(out PlayerHealth health))
                        {
                            health.TakeDirectDamage(1); // 1 Hit
                        }
                        tickTimer = 0f;
                    }
                }

                activeTimer -= Time.deltaTime;
                yield return null;
            }

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
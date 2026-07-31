using System.Collections;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject warningIndicatorPrefab;
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;
    [SerializeField] private float spawnYHeight = 10f;
    [SerializeField] private float telegraphDelay = 1.2f; // Time before rock spawns
    [SerializeField] private LayerMask groundLayer;

    [Header("Screen Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float shakeIntensity = 0.2f;

    private Coroutine spawnRoutine;

    public void StartSpawning(float interval)
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnSequence(interval));
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnSequence(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // 1. Pick a random X coordinate within the arena boundaries
            float randomX = Random.Range(leftBoundary.position.x, rightBoundary.position.x);
            Vector3 spawnPos = new Vector3(randomX, spawnYHeight, 0f);

            // 2. Raycast down to find the floor position
            RaycastHit2D groundHit = Physics2D.Raycast(spawnPos, Vector2.down, 50f, groundLayer);

            if (groundHit.collider != null)
            {
                // Spawn indicator slightly above ground contact point
                Vector3 indicatorPos = groundHit.point + new Vector2(0f, 0.05f);
                GameObject indicatorObj = Instantiate(warningIndicatorPrefab, indicatorPos, Quaternion.identity);

                if (indicatorObj.TryGetComponent<WarningIndicator>(out WarningIndicator warning))
                {
                    warning.AnimateWarning(telegraphDelay);
                }
            }

            // 3. Wait for the warning indicator duration
            yield return new WaitForSeconds(telegraphDelay);

            // 4. Drop the rock
            Instantiate(rockPrefab, spawnPos, Quaternion.identity);

            // 5. Trigger camera rumble
            StartCoroutine(DoScreenShake(0.15f));
        }
    }

    private IEnumerator DoScreenShake(float duration)
    {
        if (cameraTransform == null) yield break;

        Vector3 originalPos = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            cameraTransform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }
}

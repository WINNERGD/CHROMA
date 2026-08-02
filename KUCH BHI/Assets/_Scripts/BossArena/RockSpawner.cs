using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject warningIndicatorPrefab;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    [Header("Spawn Boundaries")]
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;
    [SerializeField] private float spawnYHeight = 12f;

    [Header("Multi-Rock Cluster Settings")]
    [SerializeField] private int minRocksPerWave = 2;
    [SerializeField] private int maxRocksPerWave = 3;
    [SerializeField] private float minDistanceBetweenRocks = 1.5f;

    [Header("Timing")]
    [SerializeField] private float telegraphDelay = 1.0f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Camera Shake Settings")]
    [SerializeField] private bool triggerCameraShake = true;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.35f;

    private Coroutine shakeCoroutine;
    private Vector3 originalCameraPos;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.localPosition;
        }
    }

    /// <summary>
    /// Triggered by BossController on Fist Stomp
    /// </summary>
    public void TriggerSingleWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        int rockCount = Random.Range(minRocksPerWave, maxRocksPerWave + 1);
        List<Vector3> waveSpawnPositions = GetUniqueSpawnPositions(rockCount);

        // 1. Rumble Camera before warning
        if (triggerCameraShake && mainCamera != null)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeCameraRoutine());
        }

        yield return new WaitForSeconds(shakeDuration);

        // 2. Spawn warning indicators
        foreach (Vector3 spawnPos in waveSpawnPositions)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(spawnPos, Vector2.down, 100f, groundLayer);

            if (groundHit.collider != null)
            {
                Vector3 indicatorPos = new Vector3(spawnPos.x, groundHit.point.y + 0.05f, 0f);
                GameObject indicator = Instantiate(warningIndicatorPrefab, indicatorPos, Quaternion.identity);

                if (indicator.TryGetComponent<WarningIndicator>(out WarningIndicator warning))
                {
                    warning.AnimateWarning(telegraphDelay);
                }
            }
        }

        // 3. Telegraph Delay
        yield return new WaitForSeconds(telegraphDelay);

        // 4. Drop Rocks
        foreach (Vector3 spawnPos in waveSpawnPositions)
        {
            Instantiate(rockPrefab, spawnPos, Quaternion.identity);
        }
    }

    private IEnumerator ShakeCameraRoutine()
    {
        float elapsed = 0f;
        originalCameraPos = mainCamera.transform.localPosition;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeIntensity;
            float offsetY = Random.Range(-1f, 1f) * shakeIntensity;

            mainCamera.transform.localPosition = new Vector3(originalCameraPos.x + offsetX, originalCameraPos.y + offsetY, originalCameraPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalCameraPos;
    }

    private List<Vector3> GetUniqueSpawnPositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();
        int maxAttempts = 20;

        for (int i = 0; i < count; i++)
        {
            float randomX = 0f;
            bool validPos = false;
            int attempts = 0;

            while (!validPos && attempts < maxAttempts)
            {
                attempts++;
                randomX = Random.Range(leftBoundary.position.x, rightBoundary.position.x);
                validPos = true;

                foreach (Vector3 existingPos in positions)
                {
                    if (Mathf.Abs(existingPos.x - randomX) < minDistanceBetweenRocks)
                    {
                        validPos = false;
                        break;
                    }
                }
            }

            positions.Add(new Vector3(randomX, spawnYHeight, 0f));
        }

        return positions;
    }
}
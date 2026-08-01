using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RockSpawner rockSpawner;
    [SerializeField] private LineRenderer laserLineRenderer;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject[] chainVisuals;

    [Header("Phase Attack Intervals")]
    [SerializeField] private float phase0Interval = 6.0f;
    [SerializeField] private float phase1Interval = 4.5f;
    [SerializeField] private float phase2Interval = 3.2f;
    [SerializeField] private float phase3Interval = 2.0f;

    [Header("Grey Laser Settings")]
    [SerializeField] private float laserChargeTime = 1.0f;
    [SerializeField] private float laserDuration = 1.2f;
    [SerializeField] private float laserDamageInterval = 0.5f; // Deals 1 hit every 0.5 seconds inside laser
    [SerializeField] private LayerMask playerLayer;

    private int buttonsPressedCount = 0;
    private float currentInterval;
    private bool isBossDefeated = false;
    private Coroutine bossLoopCoroutine;

    private void Start()
    {
        if (playerTransform == null && GameObject.FindWithTag("Player") != null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }

        currentInterval = phase0Interval;

        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = false;
            laserLineRenderer.startWidth = 0.4f;
            laserLineRenderer.endWidth = 0.4f;
            laserLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            laserLineRenderer.startColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            laserLineRenderer.endColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        }

        bossLoopCoroutine = StartCoroutine(BossAttackLoop());
    }

    private IEnumerator BossAttackLoop()
    {
        while (!isBossDefeated)
        {
            yield return new WaitForSeconds(currentInterval * 0.5f);
            yield return StartCoroutine(PerformFistStomp());

            yield return new WaitForSeconds(currentInterval * 0.5f);

            if (!isBossDefeated && playerTransform != null)
            {
                yield return StartCoroutine(FireGreyLaser());
            }
        }
    }

    private IEnumerator PerformFistStomp()
    {
        if (rockSpawner != null)
        {
            rockSpawner.TriggerSingleWave();
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator FireGreyLaser()
    {
        if (laserLineRenderer == null || laserOrigin == null) yield break;

        laserLineRenderer.enabled = true;
        laserLineRenderer.startColor = new Color(0.7f, 0.7f, 0.7f, 0.2f);
        laserLineRenderer.endColor = new Color(0.7f, 0.7f, 0.7f, 0.2f);

        float timer = 0f;
        Vector3 targetDirection = Vector3.down;

        while (timer < laserChargeTime)
        {
            timer += Time.deltaTime;
            if (playerTransform != null)
            {
                targetDirection = (playerTransform.position - laserOrigin.position).normalized;
            }

            laserLineRenderer.SetPosition(0, laserOrigin.position);
            laserLineRenderer.SetPosition(1, laserOrigin.position + targetDirection * 30f);
            yield return null;
        }

        laserLineRenderer.startColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        laserLineRenderer.endColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        timer = 0f;
        float damageTimer = 0f;

        while (timer < laserDuration)
        {
            timer += Time.deltaTime;

            Vector3 startPos = laserOrigin.position;
            Vector3 endPos = laserOrigin.position + targetDirection * 30f;

            laserLineRenderer.SetPosition(0, startPos);
            laserLineRenderer.SetPosition(1, endPos);

            RaycastHit2D hit = Physics2D.Raycast(startPos, targetDirection, 30f, playerLayer);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= laserDamageInterval)
                {
                    if (hit.collider.TryGetComponent<PlayerHealth>(out PlayerHealth health))
                    {
                        health.TakeDirectDamage(1); // 1 Hit (25%)
                    }
                    damageTimer = 0f;
                }
            }

            yield return null;
        }

        laserLineRenderer.enabled = false;
    }

    public void OnChainButtonPressed()
    {
        if (buttonsPressedCount >= 3) return;

        buttonsPressedCount++;

        if (buttonsPressedCount - 1 < chainVisuals.Length && chainVisuals[buttonsPressedCount - 1] != null)
        {
            chainVisuals[buttonsPressedCount - 1].SetActive(false);
        }

        switch (buttonsPressedCount)
        {
            case 1:
                currentInterval = phase1Interval;
                break;
            case 2:
                currentInterval = phase2Interval;
                break;
            case 3:
                currentInterval = phase3Interval;
                DefeatBoss();
                break;
        }
    }

    private void DefeatBoss()
    {
        isBossDefeated = true;
        if (bossLoopCoroutine != null) StopCoroutine(bossLoopCoroutine);
        if (laserLineRenderer != null) laserLineRenderer.enabled = false;
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("Minimum combined weight needed to activate the plate")]
    [SerializeField] private float requiredWeight = 5f;
    [SerializeField] private LayerMask triggerLayers;

    [Header("Direct Platform Targets")]
    [Tooltip("Drag your PlateActivatedPlatform scripts here to trigger them directly")]
    [SerializeField] private PlateActivatedPlatform targetPlatform;
    [SerializeField] private List<PlateActivatedPlatform> additionalPlatforms = new List<PlateActivatedPlatform>();

    [Header("Visual Feedback")]
    [SerializeField] private Transform plateVisual; // The top part of the plate that sinks down
    [SerializeField] private float sinkDistance = 0.1f;
    [SerializeField] private float sinkSpeed = 5f;

    [Header("Optional Events")]
    [SerializeField] private UnityEvent OnActivated;
    [SerializeField] private UnityEvent OnDeactivated;

    // Track unique rigidbodies on the plate to prevent double-counting mass
    private HashSet<Rigidbody2D> activeBodies = new HashSet<Rigidbody2D>();

    private float currentWeight = 0f;
    private bool isActivated = false;
    private Vector3 unpressedPos;
    private Vector3 pressedPos;

    private void Start()
    {
        if (plateVisual != null)
        {
            unpressedPos = plateVisual.localPosition;
            pressedPos = unpressedPos - new Vector3(0, sinkDistance, 0);
        }
    }

    private void Update()
    {
        // Smoothly animate the plate sinking down or raising up
        if (plateVisual != null)
        {
            Vector3 targetPos = isActivated ? pressedPos : unpressedPos;
            plateVisual.localPosition = Vector3.Lerp(plateVisual.localPosition, targetPos, Time.deltaTime * sinkSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEligible(collision))
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null && !activeBodies.Contains(rb))
            {
                activeBodies.Add(rb);
                RecalculateWeight();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsEligible(collision))
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null && activeBodies.Contains(rb))
            {
                activeBodies.Remove(rb);
                RecalculateWeight();
            }
        }
    }

    private bool IsEligible(Collider2D collision)
    {
        // Check if collision object's layer is included in triggerLayers mask
        return (triggerLayers.value & (1 << collision.gameObject.layer)) != 0;
    }

    private void RecalculateWeight()
    {
        currentWeight = 0f;

        // Sum up the mass of all unique rigidbodies currently on the plate
        foreach (Rigidbody2D rb in activeBodies)
        {
            if (rb != null)
            {
                currentWeight += rb.mass;
            }
        }

        CheckWeightThreshold();
    }

    private void CheckWeightThreshold()
    {
        if (currentWeight >= requiredWeight && !isActivated)
        {
            isActivated = true;
            ActivateTargetPlatforms();
            OnActivated?.Invoke();
            Debug.Log("Pressure Plate Activated!");
        }
        else if (currentWeight < requiredWeight && isActivated)
        {
            isActivated = false;
            DeactivateTargetPlatforms();
            OnDeactivated?.Invoke();
            Debug.Log("Pressure Plate Deactivated!");
        }
    }

    // --- DIRECT PLATFORM CONTROL FUNCTIONS ---
    public void ActivateTargetPlatforms()
    {
        // Trigger main target platform if assigned
        if (targetPlatform != null)
        {
            targetPlatform.ActivatePlatform();
        }

        // Trigger any extra platforms in the list
        foreach (var platform in additionalPlatforms)
        {
            if (platform != null)
            {
                platform.ActivatePlatform();
            }
        }
    }

    public void DeactivateTargetPlatforms()
    {
        // Deactivate main target platform
        if (targetPlatform != null)
        {
            targetPlatform.DeactivatePlatform();
        }

        // Deactivate extra platforms
        foreach (var platform in additionalPlatforms)
        {
            if (platform != null)
            {
                platform.DeactivatePlatform();
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("Minimum combined weight needed to activate the plate")]
    [SerializeField] private float requiredWeight = 5f;
    [SerializeField] private LayerMask triggerLayers;

    [Header("Visual Feedback")]
    [SerializeField] private Transform plateVisual; // The top part of the plate that sinks down
    [SerializeField] private float sinkDistance = 0.1f;
    [SerializeField] private float sinkSpeed = 5f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnActivated;
    [SerializeField] private UnityEvent OnDeactivated;

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
            if (rb != null)
            {
                currentWeight += rb.mass;
                CheckWeightThreshold();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsEligible(collision))
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null)
            {
                currentWeight -= rb.mass;
                // Prevent float precision negative numbers
                if (currentWeight < 0) currentWeight = 0;
                CheckWeightThreshold();
            }
        }
    }

    private bool IsEligible(Collider2D collision)
    {
        // Check if collision object's layer is included in triggerLayers mask
        return (triggerLayers.value & (1 << collision.gameObject.layer)) != 0;
    }

    private void CheckWeightThreshold()
    {
        if (currentWeight >= requiredWeight && !isActivated)
        {
            isActivated = true;
            OnActivated?.Invoke();
            Debug.Log("Pressure Plate Activated!");
        }
        else if (currentWeight < requiredWeight && isActivated)
        {
            isActivated = false;
            OnDeactivated?.Invoke();
            Debug.Log("Pressure Plate Deactivated!");
        }
    }
}
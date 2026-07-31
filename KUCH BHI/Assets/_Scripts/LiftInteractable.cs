using System.Collections;
using UnityEngine;

public class LiftInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform liftTransform;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 3f;

    private bool isMoving = false;

    public void Interact()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveLift());
        }
    }

    private IEnumerator MoveLift()
    {
        isMoving = true;
        Vector3 startPos = liftTransform.position;
        Vector3 endPos = targetPosition.position;
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * moveSpeed;
            liftTransform.position = Vector3.Lerp(startPos, endPos, progress);
            yield return null;
        }

        // Swap target for two-way lift functionality
        targetPosition.position = startPos;
        isMoving = false;
    }
}
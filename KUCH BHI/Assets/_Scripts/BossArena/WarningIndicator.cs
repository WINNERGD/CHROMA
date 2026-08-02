using System.Collections;
using UnityEngine;

public class WarningIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void AnimateWarning(float duration)
    {
        StartCoroutine(WarningRoutine(duration));
    }

    private IEnumerator WarningRoutine(float duration)
    {
        Vector3 startScale = new Vector3(0.1f, 0.1f, 1f);
        Vector3 targetScale = new Vector3(1.2f, 0.4f, 1f); // Flattened oval for 2D floor perspective
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Expand scale over time
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            // Optional pulse effect: blink red faster right before impact
            if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(elapsed * 8f, 0.5f) + 0.3f;
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}

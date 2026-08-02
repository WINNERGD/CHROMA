using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Remove if using Standard Pipeline

public class EnvironmentColorRestorer : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Start environment completely grey
            colorAdjustments.saturation.value = -100f;
        }
    }

    public IEnumerator TransitionToFullColorRoutine(float duration)
    {
        if (colorAdjustments == null) yield break;

        float elapsed = 0f;
        float startSaturation = colorAdjustments.saturation.value; // -100
        float targetSaturation = 0f; // Full normal color

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, elapsed / duration);
            yield return null;
        }

        colorAdjustments.saturation.value = targetSaturation;
    }
}
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Depth Settings")]
    [Tooltip("0 = Moves with Camera (Sky/Far Distant)\n0.5 = Midground\n1 = Glued to Screen (Foreground)")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectX = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectY = 0.2f;

    [Header("Infinite Scrolling (Optional)")]
    [SerializeField] private bool infiniteHorizontal = true;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        // Calculate width of sprite for seamless infinite looping
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureSizeX = spriteRenderer.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Apply parallax offset relative to layer depth factor
        float moveX = deltaMovement.x * parallaxEffectX;
        float moveY = deltaMovement.y * parallaxEffectY;

        transform.position += new Vector3(moveX, moveY, 0f);

        lastCameraPosition = cameraTransform.position;

        // Infinite Looping logic
        if (infiniteHorizontal && textureSizeX > 0)
        {
            float distanceFromCamera = cameraTransform.position.x - transform.position.x;

            if (Mathf.Abs(distanceFromCamera) >= textureSizeX)
            {
                float offsetPositionX = distanceFromCamera % textureSizeX;
                transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}
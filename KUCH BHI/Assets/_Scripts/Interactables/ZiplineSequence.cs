using System.Collections;
using UnityEngine;

public class ZiplineSequence : MonoBehaviour
{
    [Header("Interaction Prompts")]
    [SerializeField] private GameObject interactionPromptUI; // Optional 'Press E' prompt
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Sequence Parameters")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomedOutOrthographicSize = 12f;
    [SerializeField] private float cameraPanDuration = 1.5f;
    [SerializeField] private float colorRestoreDuration = 3f;
    [SerializeField] private EnvironmentColorRestorer colorRestorer;

    private bool isPlayerInZone = false;
    private bool sequenceTriggered = false;
    private PlayerScarf playerScarfRef;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerScarfRef = other.GetComponent<PlayerScarf>();
            isPlayerInZone = true;

            // Show prompt if player has collected the red crystal
            if (playerScarfRef != null && playerScarfRef.hasRedCrystal && interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInZone && !sequenceTriggered && playerScarfRef != null && playerScarfRef.hasRedCrystal)
        {
            if (Input.GetKeyDown(interactKey))
            {
                StartCoroutine(RunZiplineCutscene());
            }
        }
    }

    private IEnumerator RunZiplineCutscene()
    {
        sequenceTriggered = true;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        // 1. Disable player movement during sequence
        Rigidbody2D playerRb = playerScarfRef.GetComponent<Rigidbody2D>();
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

        // Disable player control script if present (e.g. PlayerMovement)
        MonoBehaviour movementScript = playerScarfRef.GetComponent("PlayerMovement") as MonoBehaviour;
        if (movementScript != null) movementScript.enabled = false;

        // 2. Smoothly zoom out camera
        float startCamSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < cameraPanDuration)
        {
            elapsed += Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Lerp(startCamSize, zoomedOutOrthographicSize, elapsed / cameraPanDuration);
            yield return null;
        }
        mainCamera.orthographicSize = zoomedOutOrthographicSize;

        // 3. Wait 2 seconds post-interaction before color returns
        yield return new WaitForSeconds(2.0f);

        // 4. Restore color smoothly from grey to colorful
        if (colorRestorer != null)
        {
            yield return StartCoroutine(colorRestorer.TransitionToFullColorRoutine(colorRestoreDuration));
        }

        Debug.Log("<color=green>[ZiplineSequence]</color> Sequence completed!");

        // (Optional) Re-enable movement or launch player down zipline path here
    }
}
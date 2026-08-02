using UnityEngine;

public class PlayerPushPull : MonoBehaviour
{
    [Header("Push / Pull Settings")]
    [SerializeField] private KeyCode grabKey = KeyCode.LeftShift;
    [SerializeField] private float checkDistance = 0.8f;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private Transform grabCheckPoint;

    [Header("Movement Modifier")]
    [Tooltip("Speed multiplier while pushing/pulling heavy objects")]
    //[SerializeField] private float pushSpeedMultiplier = 0.5f;

    private PushableBox currentBox;
    private bool isGrabbing = false;
    private FixedJoint2D joint;

    public bool IsGrabbing => isGrabbing;

    private void Awake()
    {
        // Add a FixedJoint2D to dynamically bind objects when pulled
        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.enabled = false;
    }

    private void Update()
    {
        // Check for Grab key input
        if (Input.GetKeyDown(grabKey))
        {
            TryGrab();
        }
        else if (Input.GetKeyUp(grabKey) && isGrabbing)
        {
            ReleaseBox();
        }
    }

    private void TryGrab()
    {
        // Raycast forward/around the grab check point to find a box
        RaycastHit2D hit = Physics2D.Raycast(
            grabCheckPoint.position,
            transform.localScale.x > 0 ? Vector2.right : Vector2.left,
            checkDistance,
            pushableLayer
        );

        if (hit.collider != null)
        {
            PushableBox box = hit.collider.GetComponent<PushableBox>();
            if (box != null)
            {
                isGrabbing = true;
                currentBox = box;

                // Lock physics joint to connect player and box rigidbodies
                joint.connectedBody = box.GetComponent<Rigidbody2D>();
                joint.enabled = true;

                box.ConnectToPlayer(transform);
            }
        }
    }

    private void ReleaseBox()
    {
        if (currentBox != null)
        {
            currentBox.DisconnectFromPlayer();
            currentBox = null;
        }

        joint.connectedBody = null;
        joint.enabled = false;
        isGrabbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (grabCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(grabCheckPoint.position, direction * checkDistance);
        }
    }
}
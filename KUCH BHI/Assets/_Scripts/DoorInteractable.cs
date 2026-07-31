using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorToOpen;
    [SerializeField] private bool destroyDoor = false;

    public void Interact()
    {
        if (doorToOpen == null) return;

        if (destroyDoor)
        {
            Destroy(doorToOpen);
        }
        else
        {
            // Toggle active state (open / close)
            doorToOpen.SetActive(!doorToOpen.activeSelf);
        }

        Debug.Log("Door toggled!");
    }
}
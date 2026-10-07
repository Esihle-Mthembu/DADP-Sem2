using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractable : MonoBehaviour
{
    public Camera playerCamera;
    public float interactRange = 3f;
    public LayerMask interactableLayer = ~0;
    public GameObject interactPromptUI;

    void Update()
    {
        DoorInteractable door = GetLookedAtDoor();

        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(door != null);
        }

        if (door != null && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            door.Interact();
        }
    }

    private DoorInteractable GetLookedAtDoor()
    {
        if (playerCamera == null)
        {
            return null;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            return hit.collider.GetComponentInParent<DoorInteractable>();
        }
        return null;
    }
}
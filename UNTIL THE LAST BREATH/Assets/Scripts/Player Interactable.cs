using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractable : MonoBehaviour
{
    public Camera playerCamera;
    public float interactRange = 6f;
    public float proximityRadius = 3f;
    public LayerMask interactableLayer = ~0;
    public GameObject interactPromptUI;
    public TMP_Text interactPromptText;

    private readonly Collider[] nearbyBuffer = new Collider[32];

    void Update()
    {
        string message = null;
        IInteractable interactable = null;

        //Prioritize object that is being looked at
        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer, QueryTriggerInteraction.Ignore))
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    message = interactable.GetPrompt();
                }
                else
                {
                    PromptTarget target = hit.collider.GetComponentInParent<PromptTarget>();
                    if (target != null) message = target.promptText;
                }
            }
        }

        //Show the nearest object around if not looking at anything with a prompt
        if (string.IsNullOrEmpty(message))
        {
            PromptTarget nearest = GetNearestPromptTarget();
            if (nearest != null)
            {
                message = nearest.promptText;
            }
        }

        bool show = !string.IsNullOrEmpty(message);
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(show);
        }

        if (show && interactPromptText != null)
        {
            interactPromptText.text = message;
        }

        if (interactable != null && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            interactable.Interact();
        }
    }

    private PromptTarget GetNearestPromptTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position, proximityRadius, nearbyBuffer,
            interactableLayer, QueryTriggerInteraction.Ignore);

        PromptTarget best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            PromptTarget t = nearbyBuffer[i].GetComponentInParent<PromptTarget>();
            if (t == null) continue;

            float d = (t.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }
        return best;
    }
}
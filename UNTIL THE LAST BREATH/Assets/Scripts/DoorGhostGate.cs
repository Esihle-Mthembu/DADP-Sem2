using UnityEngine;

public class DoorGhostGate : MonoBehaviour
{
    [SerializeField] DoorInteractable door;
    [SerializeField] Collider[] doorColliders; //Only the swinging door's solid collider

    private float entrySide;

    void Start()
    {
        door.StateChanged += Refresh;
        StartCoroutine(InitWhenGhostReady());
    }

    System.Collections.IEnumerator InitWhenGhostReady()
    {
        while (Ghost.Instance == null) yield return null;
        Refresh();
    }

    void OnDestroy()
    {
        if (door != null)
        {
            door.StateChanged -= Refresh;
        }
    }

    //The ghost passes only when the door is open and unlocked, otherwise the door blocks it
    void Refresh()
    {
        if (Ghost.Instance == null)
        {
            return;
        }

        Collider ghostCol = Ghost.Instance.GetComponent<Collider>();

        bool passable = door.IsOpen && !door.IsLocked;
        foreach (Collider c in doorColliders)
        {
            Physics.IgnoreCollision(ghostCol, c, passable);
        }
    }

    float SideOf(Vector3 worldPos)
    {
        return Mathf.Sign(Vector3.Dot(transform.forward, worldPos - transform.position));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            entrySide = SideOf(other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ghost"))
        {
            return;
        }

        //Lock only if it actually crossed to the other side, not if it backed out
        if (SideOf(other.transform.position) != entrySide)
        {
            door.ForceLock();
        }
    }
}

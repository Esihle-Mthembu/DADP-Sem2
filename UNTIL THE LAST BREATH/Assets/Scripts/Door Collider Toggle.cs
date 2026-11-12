using UnityEngine;

public class DoorColliderToggle : MonoBehaviour
{
    [SerializeField] DoorInteractable door;
    [SerializeField] Collider doorCollider;

    private Collider[] playerCols;

    void Start()
    {
        door.StateChanged += Refresh;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCols = player.transform.root.GetComponentsInChildren<Collider>();
        }

        if (playerCols == null || playerCols.Length == 0)
        {
            Debug.LogWarning("DoorColliderToggle: no player colliders found, check the Player tag", this);
        }

        Refresh();
    }

    void OnDestroy()
    {
        if (door != null)
        {
            door.StateChanged -= Refresh;
        }
    }

    void Refresh()
    {
        doorCollider.enabled = true;
        doorCollider.isTrigger = false;

        if (playerCols == null)
        {
            return;
        }

        foreach (Collider c in playerCols)
        {
            Physics.IgnoreCollision(c, doorCollider, door.IsOpen);
        }
    }
}
using UnityEngine;
using InventorySystem;

public class Collectible : MonoBehaviour, IInteractable
{
    public ItemType itemType = ItemType.Key;
    public int amount = 1;
    public string itemName = "item";
    public string pickupID; // Unique per object

    void Start()
    {
        //Remove object from the world if already collected in previous gameplay
        var sm = SaveManager.Instance;
        if (sm != null && !string.IsNullOrEmpty(pickupID) && sm.Current.collectedItems.Contains(pickupID))
        {
            Destroy(gameObject);
        }
    }

    public string GetPrompt()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.IsFull(itemType))
        {
            return "Inventory full";
        }

        return "Press E to pick up " + itemName;
    }

    public void Interact() { }

    // Called by FPController when E is pressed
    public void Collect()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        int added = InventoryManager.Instance.TryAddItem(itemType, amount);
        if (added <= 0)
        {
            return;
        }

        var sm = SaveManager.Instance;
        if (sm != null && !string.IsNullOrEmpty(pickupID) && !sm.Current.collectedItems.Contains(pickupID))
        {
            sm.Current.collectedItems.Add(pickupID);
        }

        Destroy(gameObject);
    }
}
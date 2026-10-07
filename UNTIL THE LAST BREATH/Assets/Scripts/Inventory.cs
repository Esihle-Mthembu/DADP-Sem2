using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using InventorySystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public Item keyItem;
    public Item fileItem;

    public GameObject hotbarObj;
    public GameObject inventorySlotsParent;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Dictionary<ItemType, Item> itemsByType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inventorySlots.AddRange(inventorySlotsParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);

        itemsByType = new Dictionary<ItemType, Item>
        {
            { ItemType.Key, keyItem },
            { ItemType.File, fileItem },
        };
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddItem(keyItem, ItemType.Key, 3);
        }
        else if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            AddItem(fileItem, ItemType.File, 1);
        }
    }

    public void AddItem(Item itemToAdd, ItemType type, int amount)
    {
        int accepted = InventoryManager.Instance != null
            ? InventoryManager.Instance.TryAddItem(type, amount)
            : amount;

        if (accepted <= 0) return;

        int remaining = accepted;

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0) return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if (remaining <= 0) return;
            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory is full, could not add " + remaining + " of " + itemToAdd.itemName);
        }
    }

    public int ConsumeItem(ItemType type, int amount)
    {
        if (!itemsByType.TryGetValue(type, out Item itemToRemove) || itemToRemove == null)
        {
            Debug.LogWarning($"[Inventory] No Item asset mapped for {type}.");
            return 0;
        }

        int removedFromManager = InventoryManager.Instance != null
            ? InventoryManager.Instance.RemoveItem(type, amount)
            : amount;

        if (removedFromManager <= 0) return 0;

        int remaining = removedFromManager;

        foreach (Slot slot in allSlots)
        {
            if (remaining <= 0) break;
            if (!slot.HasItem() || slot.GetItem() != itemToRemove) continue;

            int currentAmount = slot.GetAmount();
            int amountToRemove = Mathf.Min(currentAmount, remaining);
            int newAmount = currentAmount - amountToRemove;

            if (newAmount <= 0)
                slot.ClearSlot();
            else
                slot.SetItem(itemToRemove, newAmount);

            remaining -= amountToRemove;
        }

        return removedFromManager;
    }
}
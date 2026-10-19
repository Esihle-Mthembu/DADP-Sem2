using UnityEngine;
using InventorySystem;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public enum LockType { None, Pin, Puzzle, Key }

    [Header("Lock Settings")]
    public LockType lockType = LockType.None;
    public bool isUnlocked = false;

    [Header("Key Lock")]
    public ItemType requiredKeyType = ItemType.Key;
    public bool consumeKeyOnUnlock = false;
    public string lockedFeedbackMessage = "It's locked. You need a key.";

    [Header("Door Movement")]
    public Animator animator;

    [Header("Locks")]
    public PinPadUI linkedPinPad;
    public PuzzleLockUI linkedPuzzleLock;

    private bool isOpen = false;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponentInParent<Animator>();
        if (animator == null) Debug.LogWarning("DoorInteractable: no Animator found on " + name, this);
    }

    public string GetPrompt()
    {
        if (!isUnlocked)
        {
            switch (lockType)
            {
                case LockType.Pin:
                    return linkedPinPad != null
                        ? "Locked - press F to enter PIN"
                        : "Locked";

                case LockType.Puzzle:
                    return linkedPuzzleLock != null
                        ? "Locked - press F to solve the puzzle"
                        : "Locked";

                case LockType.Key:
                    bool hasKey = InventoryManager.Instance != null
                                  && InventoryManager.Instance.HasItem(requiredKeyType);
                    return hasKey
                        ? "Press F to unlock"
                        : "Locked - you need a key";
            }
        }

        return isOpen ? "Press F to close" : "Press F to open";
    }

    public void Interact()
    {
        if (!isUnlocked)
        {
            switch (lockType)
            {
                case LockType.Pin:
                    if (linkedPinPad != null) linkedPinPad.Open(this);
                    else Debug.LogWarning("DoorInteractable: Pin lock but no PinPadUI assigned on " + name, this);
                    return;

                case LockType.Puzzle:
                    if (linkedPuzzleLock != null) linkedPuzzleLock.Open(this);
                    else Debug.LogWarning("DoorInteractable: Puzzle lock but no PuzzleLockUI assigned on " + name, this);
                    return;

                case LockType.Key:
                    TryOpenWithKey();
                    return;
            }
        }

        ToggleDoor();
    }

    private void TryOpenWithKey()
    {
        bool hasKey = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredKeyType);

        if (!hasKey)
        {
            Debug.Log(lockedFeedbackMessage);
            return;
        }

        if (consumeKeyOnUnlock && Inventory.Instance != null)
        {
            Inventory.Instance.ConsumeItem(requiredKeyType, 1);
        }

        Unlock();
    }

    private void ToggleDoor()
    {
        if (animator == null) return;

        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    public void Unlock()
    {
        isUnlocked = true;
        ToggleDoor();
    }
}
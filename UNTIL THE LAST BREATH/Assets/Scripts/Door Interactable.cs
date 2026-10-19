using UnityEngine;
using InventorySystem;

public class DoorInteractable : MonoBehaviour
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

    public void Interact()
    {
        if (!isUnlocked && lockType == LockType.Pin && linkedPinPad != null)
        {
            linkedPinPad.Open(this);
            return;
        }

        if (!isUnlocked && lockType == LockType.Puzzle && linkedPuzzleLock != null)
        {
            linkedPuzzleLock.Open(this);
            return;
        }

        if (!isUnlocked && lockType == LockType.Key)
        {
            TryOpenWithKey();
            return;
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
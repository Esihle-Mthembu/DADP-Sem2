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
    public Transform doorTransform;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Locks")]
    public PinPadUI linkedPinPad;
    public PuzzleLockUI linkedPuzzleLock;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        closedRotation = doorTransform.localRotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
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
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private System.Collections.IEnumerator RotateDoor(Quaternion target)
    {
        while (Quaternion.Angle(doorTransform.localRotation, target) > 0.5f)
        {
            doorTransform.localRotation = Quaternion.Slerp(
                doorTransform.localRotation, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        doorTransform.localRotation = target;
    }

    public void Unlock()
    {
        isUnlocked = true;
        ToggleDoor();
    }
}
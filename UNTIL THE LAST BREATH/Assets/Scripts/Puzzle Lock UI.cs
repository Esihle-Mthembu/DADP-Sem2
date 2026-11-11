using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleLockUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject puzzleCanvas;
    [SerializeField] private PuzzleGridController puzzleGrid;

    private DoorInteractable currentDoor;
    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(false);
        }

        if (puzzleGrid != null)
        {
            puzzleGrid.OnSolved += HandleSolved;
        }
    }

    private void OnDestroy()
    {
        if (puzzleGrid != null)
        {
            puzzleGrid.OnSolved -= HandleSolved;
        }

        if (isOpen) 
        { 
            isOpen = false; UIState.Pop(); 
        }
    }

    public void Open(DoorInteractable door)
    {
        if (isOpen)
        {
            return;
        }

        currentDoor = door;
        isOpen = true;

        puzzleCanvas.SetActive(true);
        puzzleGrid.enabled = true;

        UIState.Push();
    }

    public void Close()
    {
        if (!isOpen) return;  
        CancelInvoke(nameof(Close));

        isOpen = false;
        puzzleCanvas.SetActive(false);
        currentDoor = null;

        UIState.Pop();
    }

    private void HandleSolved()
    {
        currentDoor?.Unlock();
        Invoke(nameof(Close), 1f);
    }

    private void Update()
    {
        if (isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }
}
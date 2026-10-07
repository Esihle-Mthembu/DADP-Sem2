using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleLockUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject puzzleCanvas;
    [SerializeField] private PuzzleGridController puzzleGrid;

    [Header("Player Control Lock (assign your FP scripts)")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableWhileSolving;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorWhileOpen = true;

    private DoorInteractable currentDoor;
    private bool isOpen = false;

    private void Awake()
    {
        if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
        if (puzzleGrid != null) puzzleGrid.OnSolved += HandleSolved;
    }

    public void Open(DoorInteractable door)
    {
        currentDoor = door;
        isOpen = true;

        puzzleCanvas.SetActive(true);
        puzzleGrid.enabled = true;

        if (lockCursorWhileOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        SetPlayerControlEnabled(false);
    }

    public void Close()
    {
        isOpen = false;
        puzzleCanvas.SetActive(false);
        currentDoor = null;

        if (lockCursorWhileOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        SetPlayerControlEnabled(true);
    }

    private void SetPlayerControlEnabled(bool enabled)
    {
        foreach (var script in scriptsToDisableWhileSolving)
            if (script != null) script.enabled = enabled;
    }

    private void HandleSolved()
    {
        currentDoor?.Unlock();
        Invoke(nameof(Close), 1f); // let the player see the completed picture
    }

    private void Update()
    {
        if (isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }
}
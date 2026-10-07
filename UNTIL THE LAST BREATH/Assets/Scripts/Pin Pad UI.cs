using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PinPadUI : MonoBehaviour
{
    [Header("Setup")]
    public GameObject panelRoot;
    public TMP_Text displayText;
    public string correctPin = "1234";
    public int pinLength = 4;

    [Header("Feedback")]
    public TMP_Text feedbackText;      
    public float wrongPinResetDelay = 0.6f;

    [Header("Player Control")]
    public MonoBehaviour playerInputScript; 
    public bool lockCursorWhileOpen = false;

    private string enteredDigits = "";
    private DoorInteractable currentDoor;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void Open(DoorInteractable door)
    {
        currentDoor = door;
        enteredDigits = "";
        UpdateDisplay();
        if (feedbackText != null) feedbackText.text = "";

        panelRoot.SetActive(true);

        if (playerInputScript != null)
        {
            playerInputScript.enabled = false;
        }

        Cursor.lockState = lockCursorWhileOpen ? CursorLockMode.None : Cursor.lockState;
        Cursor.visible = lockCursorWhileOpen ? true : Cursor.visible;
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        currentDoor = null;

        if (playerInputScript != null)
        {
            playerInputScript.enabled = true;
        }

        Cursor.lockState = lockCursorWhileOpen ? CursorLockMode.Locked : Cursor.lockState;
        Cursor.visible = lockCursorWhileOpen ? false : Cursor.visible;
    }

    public void OnDigitPressed(int digit)
    {
        if (enteredDigits.Length >= pinLength) return;

        enteredDigits += digit.ToString();
        UpdateDisplay();

        if (enteredDigits.Length == pinLength)
        {
            CheckPin();
        }
    }

    public void OnClear()
    {
        enteredDigits = "";
        UpdateDisplay();
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    private void UpdateDisplay()
    {
        if (displayText == null)
        {
            return;
        }

        string shown = enteredDigits.PadRight(pinLength, '_');
        displayText.text = string.Join(" ", shown.ToCharArray());
    }

    private void CheckPin()
    {
        if (enteredDigits == correctPin)
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Unlocked";
            }

            currentDoor?.Unlock();
            Invoke(nameof(Close), 0.3f);
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Incorrect code";
            }

            Invoke(nameof(OnClear), wrongPinResetDelay);
        }
    }

    void Update()
    {
        if (panelRoot != null && panelRoot.activeSelf && Keyboard.current != null && Keyboard.current.altKey.wasPressedThisFrame)
        {
            Close();
        }
    }
}

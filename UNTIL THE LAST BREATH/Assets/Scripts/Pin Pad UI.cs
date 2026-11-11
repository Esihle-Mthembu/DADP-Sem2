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

    private string enteredDigits = "";
    private DoorInteractable currentDoor;
    private bool isOpen = false;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void Open(DoorInteractable door)
    {
        if (isOpen) return;
        isOpen = true;

        currentDoor = door;
        enteredDigits = "";
        UpdateDisplay();

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        panelRoot.SetActive(true);
        UIState.Push();
    }

    public void Close()
    {
        if (!isOpen) return;
        CancelInvoke();

        isOpen = false;
        panelRoot.SetActive(false);
        currentDoor = null;

        UIState.Pop();
    }

    private void OnDestroy()
    {
        if (isOpen)
        {
            isOpen = false;
            UIState.Pop();
        }
    }

    public void OnDigitPressed(int digit)
    {
        if (enteredDigits.Length >= pinLength)
        {
            return;
        }

        enteredDigits += digit.ToString();
        UpdateDisplay();

        if (enteredDigits.Length == pinLength)
        {
            CheckPin();
        }
    }

    public void OnClear()
    {
        Debug.Log("Close pressed");

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

    private static readonly Key[] digitKeys =
    {
    Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };

    private static readonly Key[] numpadKeys =
    {
    Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4, Key.Numpad5, Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9
    };

    void Update()
    {
        if (!isOpen)
        {
            return;
        }

        var kb = Keyboard.current;
        if (kb == null)
        {
            return;
        }

        if (kb.altKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame)
        {
            Close();
            return;
        }

        for (int i = 0; i <= 9; i++)
        {
            if (kb[digitKeys[i]].wasPressedThisFrame || kb[numpadKeys[i]].wasPressedThisFrame)
            {
                OnDigitPressed(i);
            }
        }

        if (kb.backspaceKey.wasPressedThisFrame)
        {
            OnBackspace();
        }
    }

    public void OnBackspace()
    {
        if (enteredDigits.Length == 0)
        {
            return;
        }

        enteredDigits = enteredDigits.Substring(0, enteredDigits.Length - 1);
        UpdateDisplay();
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
}
